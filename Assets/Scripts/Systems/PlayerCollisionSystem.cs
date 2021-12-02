using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

public class PlayerCollisionSystem : JobComponentSystem {
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate() {
        base.OnCreate();

        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    [BurstCompile]
    struct PlayerCollisionSystemJob : ITriggerEventsJob {
        [ReadOnly] public ComponentDataFromEntity<MoveComponentData> allMovables;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> allPlayers;
        public EntityCommandBuffer commandBuffer;

        public void Execute( TriggerEvent triggerEvent ) {
            Entity a = triggerEvent.EntityA;
            Entity b = triggerEvent.EntityB;

            if ( allPlayers.HasComponent( a ) ) {
                OnCollisionStay( b, ref commandBuffer );
            } else if ( allPlayers.HasComponent( b ) ) {
                OnCollisionStay( b, ref commandBuffer );
            }
        }

        public void OnCollisionStay( Entity other, ref EntityCommandBuffer commandBuffer ) {
            commandBuffer.DestroyEntity( other );
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps ) {
        var job = new PlayerCollisionSystemJob();
        job.allMovables = GetComponentDataFromEntity<MoveComponentData>( true );
        job.allPlayers = GetComponentDataFromEntity<PlayerTag>( true );
        job.commandBuffer = commandBufferSystem.CreateCommandBuffer();

        JobHandle handle = job.Schedule( stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps );
        //handle.Complete();
        commandBufferSystem.AddJobHandleForProducer( handle );
        return handle;
    }

}
