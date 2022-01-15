using ECS.Scripts.ComponentData;

namespace ECS.Scripts.ComponentSystem
{
    public class LevelUpSystem : Unity.Entities.ComponentSystem
    {
        
        protected override void OnUpdate()
        {
            Entities.ForEach((ref LevelComponent levelComponent) => { levelComponent.Level += 1f * Time.DeltaTime; });
        }
        
    }
}