'use client';

import { Canvas } from '@react-three/fiber';
import { Environment, Html, OrbitControls } from '@react-three/drei';
import { Suspense, useState } from 'react';
import type { VrScene } from '@/lib/api';

interface Props {
  scene: VrScene;
}

function FurnitureMesh({
  item,
  onSelect,
  selected,
}: {
  item: VrScene['furnitureItems'][0];
  onSelect: () => void;
  selected: boolean;
}) {
  const { x, y, z } = item.position;
  return (
    <group position={[x, y, z]}>
      <mesh
        onClick={(e) => {
          e.stopPropagation();
          onSelect();
        }}
        onPointerOver={() => (document.body.style.cursor = 'pointer')}
        onPointerOut={() => (document.body.style.cursor = 'default')}
      >
        <boxGeometry args={[1.2, 0.8, 0.6]} />
        <meshStandardMaterial
          color={selected ? '#d97706' : '#a8a29e'}
          metalness={0.2}
          roughness={0.6}
        />
      </mesh>
      <Html distanceFactor={8} position={[0, 1, 0]}>
        <div className="whitespace-nowrap rounded bg-black/70 px-2 py-1 text-xs text-white">
          {item.name}
        </div>
      </Html>
    </group>
  );
}

function ShowroomScene({
  scene,
  selectedId,
  onSelect,
}: {
  scene: VrScene;
  selectedId: string | null;
  onSelect: (id: string) => void;
}) {
  return (
    <>
      <ambientLight intensity={0.4} />
      <directionalLight position={[5, 8, 5]} intensity={1} castShadow />
      <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, -0.01, 0]} receiveShadow>
        <planeGeometry args={[20, 20]} />
        <meshStandardMaterial color="#e7e5e4" />
      </mesh>
      <mesh position={[0, 1.5, -4]}>
        <boxGeometry args={[8, 3, 0.2]} />
        <meshStandardMaterial color="#d6d3d1" />
      </mesh>
      {scene.furnitureItems.map((item) => (
        <FurnitureMesh
          key={item.id}
          item={item}
          selected={selectedId === item.id}
          onSelect={() => onSelect(item.id)}
        />
      ))}
      {scene.hdrUrl && (
        <Environment files={scene.hdrUrl} background blur={0.6} />
      )}
      <OrbitControls
        enablePan
        enableZoom
        minDistance={2}
        maxDistance={12}
        maxPolarAngle={Math.PI / 2.1}
      />
    </>
  );
}

export function KitchenVrShowroom({ scene }: Props) {
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const selected = scene.furnitureItems.find((f) => f.id === selectedId);

  return (
    <div className="relative h-[calc(100vh-80px)] w-full bg-stone-900">
      <Canvas shadows camera={{ position: [4, 3, 6], fov: 50 }}>
        <Suspense fallback={null}>
          <ShowroomScene
            scene={scene}
            selectedId={selectedId}
            onSelect={setSelectedId}
          />
        </Suspense>
      </Canvas>

      <div className="pointer-events-none absolute left-4 top-4 rounded-xl bg-black/60 px-4 py-3 text-white backdrop-blur">
        <p className="text-xs uppercase tracking-widest text-amber-300">VR Showroom</p>
        <h2 className="font-serif text-xl">{scene.name}</h2>
        <p className="mt-1 text-xs text-stone-300">
          Drag to orbit · Scroll to zoom · Click furniture
        </p>
      </div>

      {selected && (
        <aside className="absolute right-4 top-4 w-80 rounded-2xl bg-white p-6 shadow-2xl">
          <button
            onClick={() => setSelectedId(null)}
            className="absolute right-4 top-4 text-stone-400 hover:text-stone-700"
          >
            ✕
          </button>
          <p className="text-xs uppercase tracking-widest text-amber-700">Product</p>
          <h3 className="mt-1 text-xl font-semibold">{selected.name}</h3>
          <p className="mt-2 text-sm text-stone-600">{selected.description}</p>
          <dl className="mt-4 space-y-2 text-sm">
            {selected.material && (
              <div className="flex justify-between">
                <dt className="text-stone-500">Material</dt>
                <dd>{selected.material}</dd>
              </div>
            )}
            {selected.dimensions && (
              <div className="flex justify-between">
                <dt className="text-stone-500">Dimensions</dt>
                <dd>{selected.dimensions}</dd>
              </div>
            )}
          </dl>
          <p className="mt-4 text-2xl font-semibold text-amber-800">
            ${Number(selected.price).toLocaleString()}
          </p>
        </aside>
      )}
    </div>
  );
}
