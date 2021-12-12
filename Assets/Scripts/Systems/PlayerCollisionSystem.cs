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
        public ComponentDataFromEntity<MoveComponentData> allMovables;
        public ComponentDataFromEntity<PlayerTag> allPlayers;

        public EntityCommandBuffer commandBuffer;

        public void Execute( TriggerEvent triggerEvent ) {
            Entity a = triggerEvent.EntityA;
            Entity b = triggerEvent.EntityB;

            if ( allPlayers.HasComponent( a ) ) {
                OnTriggerStay( a, b, ref commandBuffer );
            } else if ( allPlayers.HasComponent( b ) ) {
                OnTriggerStay( b, a, ref commandBuffer );
            }
        }

        public void OnTriggerStay( Entity _this, Entity other, ref EntityCommandBuffer commandBuffer ) {
            commandBuffer.DestroyEntity( other );
            MoveComponentData m = allMovables[_this];
            m.destroyed_obj_count += 1;
            allMovables[_this] = m;
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps ) {
        var job = new PlayerCollisionSystemJob();
        job.allMovables = GetComponentDataFromEntity<MoveComponentData>( false );
        job.allPlayers = GetComponentDataFromEntity<PlayerTag>( false );
        job.commandBuffer = commandBufferSystem.CreateCommandBuffer();

        JobHandle handle = job.Schedule( stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps );
        //handle.Complete();
        commandBufferSystem.AddJobHandleForProducer( handle );
        return handle;
    }
}
