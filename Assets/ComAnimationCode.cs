using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComAnimationCode : MonoBehaviour
{
    private Transform root;
    private Transform upRightArm, lowRightArm;
    private Transform upLeftArm, lowLeftArm;
    private Transform upRightLeg, lowRightLeg;
    private Transform upLeftLeg, lowLeftLeg;
    private Transform neck;
    private Transform wristRight, wristLeft;
    private Transform footRight, footLeft;
    Animator animator;
    public Animator StandardAni;
    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();
        root=animator.GetBoneTransform(HumanBodyBones.Hips);
        upRightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        lowRightArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        upLeftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        lowLeftArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        upRightLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        lowRightLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        upLeftLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        lowLeftLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        neck = animator.GetBoneTransform(HumanBodyBones.Jaw);
        wristRight = animator.GetBoneTransform(HumanBodyBones.RightHand);
        wristLeft = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        footRight = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        footLeft = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
    }

    // Update is called once per frame
    void Update()
    {

        UpdateBone();
    }
    

    private void UpdateBone()
    {
        root.rotation = StandardAni.GetBoneTransform(HumanBodyBones.Hips).rotation * root.rotation;

        upRightArm.rotation = StandardAni.GetBoneTransform(HumanBodyBones.RightUpperArm).rotation * upRightArm.rotation;
        lowRightArm.rotation = StandardAni.GetBoneTransform(HumanBodyBones.RightLowerArm).rotation * lowRightArm.rotation;

        upLeftArm.rotation = StandardAni.GetBoneTransform(HumanBodyBones.LeftUpperArm).rotation * upLeftArm.rotation;
        lowLeftArm.rotation = StandardAni.GetBoneTransform(HumanBodyBones.LeftLowerArm).rotation * lowLeftArm.rotation;

        upRightLeg.rotation = StandardAni.GetBoneTransform(HumanBodyBones.RightUpperLeg).rotation * upRightLeg.rotation;
        lowRightLeg.rotation = StandardAni.GetBoneTransform(HumanBodyBones.RightLowerLeg).rotation * lowRightLeg.rotation;

        upLeftLeg.rotation = StandardAni.GetBoneTransform(HumanBodyBones.LeftUpperLeg).rotation * upLeftLeg.rotation;
        lowLeftLeg.rotation = StandardAni.GetBoneTransform(HumanBodyBones.LeftLowerLeg).rotation * lowLeftLeg.rotation;

        neck.rotation = StandardAni.GetBoneTransform(HumanBodyBones.Jaw).rotation * neck.rotation;

        wristRight.rotation = StandardAni.GetBoneTransform(HumanBodyBones.RightHand).rotation * wristRight.rotation;
        wristLeft.rotation = StandardAni.GetBoneTransform(HumanBodyBones.LeftHand).rotation * wristLeft.rotation;

        footRight.rotation = StandardAni.GetBoneTransform(HumanBodyBones.RightFoot).rotation * footRight.rotation;
        footLeft.rotation = StandardAni.GetBoneTransform(HumanBodyBones.LeftFoot).rotation * footLeft.rotation;
    }
}
