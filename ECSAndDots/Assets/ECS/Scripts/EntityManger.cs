using Unity.Entities;
using UnityEngine;
using ECS.Scripts.ComponentData;
using Unity.Collections;
using Unity.Rendering;
using Unity.Transforms;

namespace ECS.Scripts
{
    public class EntityManger : MonoBehaviour
    {

        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        
        private void Start()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(LevelComponent),
                typeof(Translation),
                typeof(RenderMesh),
                typeof(LocalToWorld)
            );

            NativeArray<Entity> entityArray = new NativeArray<Entity>(2, Allocator.Temp);
            entityManager.CreateEntity(entityArchetype, entityArray);
            
            foreach (var entity in entityArray)
            {
                entityManager.SetComponentData(entity, new LevelComponent() { Level = Random.Range(10,20) });
                entityManager.SetSharedComponentData(entity, new RenderMesh() { mesh = mesh, material = material });
            }
        }
        
    }
}
