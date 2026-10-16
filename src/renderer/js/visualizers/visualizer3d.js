/**
 * GeoFluent 3D WebGL Visualizer
 * High-performance 3D scene using Three.js with Fluent Design glassmorphic aesthetics,
 * interactive orbital controls, lighting, wireframe overlay, and dynamic geometric mesh generation.
 */

import * as THREE from 'three';

export class Visualizer3D {
  constructor(containerElement) {
    this.container = containerElement;
    this.scene = null;
    this.camera = null;
    this.renderer = null;
    this.currentMeshGroup = null;
    this.gridHelper = null;
    this.animId = null;

    this.isAutoRotating = true;
    this.showWireframe = true;

    // Orbit control state
    this.isDragging = false;
    this.prevMousePos = { x: 0, y: 0 };
    this.rotationVelocity = { x: 0.002, y: 0.005 };

    this._init();
  }

  _init() {
    if (!this.container) return;

    const width = this.container.clientWidth || 400;
    const height = this.container.clientHeight || 300;

    // 1. Scene
    this.scene = new THREE.Scene();

    // 2. Camera
    this.camera = new THREE.PerspectiveCamera(45, width / height, 0.1, 1000);
    this.camera.position.set(20, 15, 25);
    this.camera.lookAt(0, 0, 0);

    // 3. Renderer
    this.renderer = new THREE.WebGLRenderer({
      antialias: true,
      alpha: true,
      powerPreference: 'high-performance'
    });
    this.renderer.setSize(width, height);
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFShadowMap;

    this.container.innerHTML = '';
    this.container.appendChild(this.renderer.domElement);

    // 4. Lights
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.7);
    this.scene.add(ambientLight);

    const dirLight1 = new THREE.DirectionalLight(0x60cdff, 1.2);
    dirLight1.position.set(20, 30, 20);
    dirLight1.castShadow = true;
    this.scene.add(dirLight1);

    const dirLight2 = new THREE.DirectionalLight(0x0078d4, 0.8);
    dirLight2.position.set(-20, -10, -20);
    this.scene.add(dirLight2);

    // 5. Grid Helper
    this.gridHelper = new THREE.GridHelper(20, 20, 0x0078d4, 0x223344);
    this.gridHelper.position.y = -6;
    this.scene.add(this.gridHelper);

    // 6. Mouse Interaction
    this._attachMouseEvents();

    // 7. Resize Observer
    this._attachResizeObserver();

    // 8. Start Loop
    this._animate();
  }

  _attachMouseEvents() {
    const dom = this.renderer.domElement;

    dom.addEventListener('mousedown', (e) => {
      this.isDragging = true;
      this.prevMousePos = { x: e.clientX, y: e.clientY };
    });

    window.addEventListener('mouseup', () => {
      this.isDragging = false;
    });

    window.addEventListener('mousemove', (e) => {
      if (!this.isDragging || !this.currentMeshGroup) return;
      const deltaX = e.clientX - this.prevMousePos.x;
      const deltaY = e.clientY - this.prevMousePos.y;

      this.currentMeshGroup.rotation.y += deltaX * 0.008;
      this.currentMeshGroup.rotation.x += deltaY * 0.008;

      this.prevMousePos = { x: e.clientX, y: e.clientY };
    });

    dom.addEventListener('wheel', (e) => {
      e.preventDefault();
      const zoomFactor = e.deltaY > 0 ? 1.08 : 0.92;
      this.camera.position.multiplyScalar(zoomFactor);
      this.camera.position.clampLength(5, 100);
      this.camera.lookAt(0, 0, 0);
    }, { passive: false });
  }

  _attachResizeObserver() {
    const ro = new ResizeObserver(() => {
      if (!this.container || !this.renderer || !this.camera) return;
      const w = this.container.clientWidth;
      const h = this.container.clientHeight;
      if (w > 0 && h > 0) {
        this.camera.aspect = w / h;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(w, h);
      }
    });
    ro.observe(this.container);
  }

  setAutoRotate(enabled) {
    this.isAutoRotating = enabled;
  }

  setWireframe(enabled) {
    this.showWireframe = enabled;
    if (this.currentMeshGroup) {
      this.currentMeshGroup.traverse((child) => {
        if (child.isLineSegments) {
          child.visible = enabled;
        }
      });
    }
  }

  resetView() {
    this.camera.position.set(20, 15, 25);
    this.camera.lookAt(0, 0, 0);
    if (this.currentMeshGroup) {
      this.currentMeshGroup.rotation.set(0, 0, 0);
    }
  }

  render(threeData) {
    if (!this.scene) return;

    // Remove previous mesh
    if (this.currentMeshGroup) {
      this.scene.remove(this.currentMeshGroup);
      this.currentMeshGroup.traverse((child) => {
        if (child.geometry) child.geometry.dispose();
        if (child.material) {
          if (Array.isArray(child.material)) child.material.forEach(m => m.dispose());
          else child.material.dispose();
        }
      });
      this.currentMeshGroup = null;
    }

    if (!threeData) return;

    this.currentMeshGroup = new THREE.Group();

    // Scale normalizer so shapes render cleanly in the 10-unit box
    let geometry;
    const norm = 6; // target size

    switch (threeData.type) {
      case 'sphere': {
        geometry = new THREE.SphereGeometry(norm, 48, 36);
        break;
      }
      case 'hemisphere': {
        geometry = new THREE.SphereGeometry(norm, 48, 24, 0, Math.PI * 2, 0, Math.PI / 2);
        // Add flat circular base
        const capGeom = new THREE.CircleGeometry(norm, 48);
        capGeom.rotateX(Math.PI / 2);
        const cap = new THREE.Mesh(capGeom, this._createSolidMaterial());
        this.currentMeshGroup.add(cap);
        break;
      }
      case 'cube': {
        geometry = new THREE.BoxGeometry(norm * 1.5, norm * 1.5, norm * 1.5);
        break;
      }
      case 'cuboid': {
        const maxDim = Math.max(threeData.width, threeData.height, threeData.depth) || 1;
        const scale = (norm * 1.8) / maxDim;
        geometry = new THREE.BoxGeometry(
          threeData.width * scale,
          threeData.height * scale,
          threeData.depth * scale
        );
        break;
      }
      case 'cylinder': {
        const maxDim = Math.max(threeData.radius * 2, threeData.height) || 1;
        const scale = (norm * 1.8) / maxDim;
        geometry = new THREE.CylinderGeometry(
          threeData.radius * scale,
          threeData.radius * scale,
          threeData.height * scale,
          36
        );
        break;
      }
      case 'cone': {
        const maxDim = Math.max(threeData.radius * 2, threeData.height) || 1;
        const scale = (norm * 1.8) / maxDim;
        geometry = new THREE.ConeGeometry(
          threeData.radius * scale,
          threeData.height * scale,
          36
        );
        break;
      }
      case 'frustum': {
        const maxDim = Math.max(threeData.radiusBottom * 2, threeData.height) || 1;
        const scale = (norm * 1.8) / maxDim;
        geometry = new THREE.CylinderGeometry(
          threeData.radiusTop * scale,
          threeData.radiusBottom * scale,
          threeData.height * scale,
          36
        );
        break;
      }
      case 'pyramid': {
        const maxDim = Math.max(threeData.baseSize, threeData.height) || 1;
        const scale = (norm * 1.8) / maxDim;
        const r = (threeData.baseSize * scale) / Math.SQRT2;
        geometry = new THREE.ConeGeometry(r, threeData.height * scale, 4);
        geometry.rotateY(Math.PI / 4);
        break;
      }
      case 'prism': {
        const maxDim = Math.max(threeData.baseA, threeData.length) || 1;
        const scale = (norm * 1.8) / maxDim;
        const r = (threeData.baseA * scale) / Math.sqrt(3);
        geometry = new THREE.CylinderGeometry(r, r, threeData.length * scale, 3);
        geometry.rotateX(Math.PI / 2);
        break;
      }
      case 'torus': {
        const maxDim = (threeData.radius + threeData.tube) * 2 || 1;
        const scale = (norm * 2) / maxDim;
        geometry = new THREE.TorusGeometry(
          threeData.radius * scale,
          threeData.tube * scale,
          24,
          48
        );
        geometry.rotateX(Math.PI / 3);
        break;
      }
      case 'ellipsoid': {
        const maxDim = Math.max(threeData.a, threeData.b, threeData.c) || 1;
        const scale = norm / maxDim;
        geometry = new THREE.SphereGeometry(1, 48, 36);
        geometry.scale(threeData.a * scale, threeData.b * scale, threeData.c * scale);
        break;
      }
      case 'capsule': {
        const maxDim = Math.max(threeData.radius * 2, threeData.length + 2 * threeData.radius) || 1;
        const scale = (norm * 1.8) / maxDim;
        geometry = new THREE.CapsuleGeometry(
          threeData.radius * scale,
          threeData.length * scale,
          12,
          32
        );
        break;
      }
      default:
        geometry = new THREE.SphereGeometry(norm, 32, 24);
    }

    // 1. Fluent acrylic solid mesh
    const solidMesh = new THREE.Mesh(geometry, this._createSolidMaterial());
    solidMesh.castShadow = true;
    solidMesh.receiveShadow = true;
    this.currentMeshGroup.add(solidMesh);

    // 2. Wireframe / Edge Highlight
    const wireGeom = new THREE.EdgesGeometry(geometry, 25);
    const wireMat = new THREE.LineBasicMaterial({
      color: 0x60cdff,
      linewidth: 1.5,
      transparent: true,
      opacity: 0.85
    });
    const wireMesh = new THREE.LineSegments(wireGeom, wireMat);
    wireMesh.visible = this.showWireframe;
    this.currentMeshGroup.add(wireMesh);

    this.scene.add(this.currentMeshGroup);
  }

  _createSolidMaterial() {
    return new THREE.MeshPhysicalMaterial({
      color: 0x0078d4,
      emissive: 0x001d3d,
      roughness: 0.15,
      metalness: 0.1,
      transmission: 0.4,
      transparent: true,
      opacity: 0.88,
      reflectivity: 0.8,
      clearcoat: 0.6,
      clearcoatRoughness: 0.1,
      side: THREE.DoubleSide
    });
  }

  _animate() {
    this.animId = requestAnimationFrame(() => this._animate());

    if (this.currentMeshGroup && this.isAutoRotating && !this.isDragging) {
      this.currentMeshGroup.rotation.y += 0.006;
    }

    if (this.renderer && this.scene && this.camera) {
      this.renderer.render(this.scene, this.camera);
    }
  }

  destroy() {
    if (this.animId) cancelAnimationFrame(this.animId);
    if (this.renderer) {
      this.renderer.dispose();
      if (this.renderer.domElement && this.renderer.domElement.parentNode) {
        this.renderer.domElement.parentNode.removeChild(this.renderer.domElement);
      }
    }
  }
}
