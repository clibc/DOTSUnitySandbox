using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore( typeof( MoveSystem ) )]
public class PlayerInputSystem : SystemBase {
    protected override void OnUpdate() {
        Entities
        .WithAny<PlayerTag>()
        .ForEach( ( ref MoveComponentData moveData ) => {
            float3 dir = float3.zero;
            if ( Input.GetKey( KeyCode.A ) ) {
                dir.x = -1.0f;
            }
            if ( Input.GetKey( KeyCode.D ) ) {
                dir.x = 1.0f;
            }
            if ( Input.GetKey( KeyCode.W ) ) {
                dir.z = 1.0f;
            }
            if ( Input.GetKey( KeyCode.S ) ) {
                dir.z = -1.0f;
            }

            moveData.move_dir = dir;

        } ).Run();
    }
}
