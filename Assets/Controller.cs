using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    public Transform GoalBone;
    public Vector3 ChildBonePos, Forward;
    public Quaternion MidMat;
    public Controller(Transform goalBone, Vector3 childBonePos, Vector3 forward)
    {
        GoalBone = goalBone;
        ChildBonePos = childBonePos;
        Forward = forward;
        MatInit();
    }

    private void MatInit()
    {
        MidMat=Quaternion.Inverse(GoalBone.rotation) * Quaternion.LookRotation(ChildBonePos-GoalBone.position,Forward);
    }

    public void UpdateTransform(Vector3 forward, Vector3 upward)
    {
        GoalBone.rotation = Quaternion.LookRotation((Vector3)upward, forward) * Quaternion.Inverse(MidMat);
    }
}
