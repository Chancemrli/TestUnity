using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using System.Threading;
using UnityEngine.UIElements;

public class AnimationCode : MonoBehaviour
{
    public GameObject[] Body;

    private Transform root, spine, chest;
    private Transform upRightArm, lowRightArm;
    private Transform upLeftArm, lowLeftArm;
    private Transform upRightLeg, lowRightLeg;
    private Transform upLeftLeg, lowLeftLeg;
    private Transform leftToe,rightToe;
    private Transform neck;
    private Transform wristRight, wristLeft;
    private Transform footRight, footLeft;
    private Transform lshoulder, rshoulder;
    private Transform lmidFinger, rmidFinger;
    private Transform Head;
    private Transform leftEye, rightEye;
    Animator animator;
    List<string> lines;
    public UDPReceive udpManager;
    int counter = 0;
    int m = 0;
    private Vector3 Forward;
    Controller RootCon, SpineCon, ChestCon,
        LeftShoulder, UpLArmCon, LowLArmCon,
        RightShoulder, UpRArmCon, LowRArmCon,
        UpLLegCon, LKneeCon, LeftFootCon,
        UpRLegCon, RKneeCon, RightFootCon,
        LeftHandCon, RightHandCon,
        NeckCon, HeadCon;

    private void InitController()
    {
        //���ɲ���
        RootCon = new Controller(root, chest.position, Forward);
        SpineCon = new Controller(spine, chest.position, Forward);
        ChestCon = new Controller(chest, neck.position, Forward);

        //���
        LeftShoulder = new Controller(lshoulder, upLeftArm.position, Forward);
        UpLArmCon = new Controller(upLeftArm, lowLeftArm.position, Forward);
        LowLArmCon = new Controller(lowLeftArm, wristLeft.position, Forward);
        //����
        LeftHandCon = new Controller(wristLeft, lmidFinger.position, Forward);

        //�ұ�
        RightShoulder = new Controller(rshoulder, upRightArm.position, Forward);
        UpRArmCon = new Controller(upRightArm, lowRightArm.position, Forward);
        LowRArmCon = new Controller(lowRightArm, wristRight.position, Forward);
        //����
        RightHandCon = new Controller(wristRight,rmidFinger.position,Forward);

        //����
        UpLLegCon = new Controller(upLeftLeg, lowLeftLeg.position, Forward);
        LKneeCon = new Controller(lowLeftLeg, footLeft.position, Forward);
        LeftFootCon = new Controller(footLeft, leftToe.position, footLeft.position-lowLeftLeg.position);

        //����
        UpRLegCon = new Controller(upRightLeg, lowRightLeg.position, Forward);
        RKneeCon = new Controller(lowRightLeg, footRight.position, Forward);
        RightFootCon = new Controller(footRight, rightToe.position, footRight.position-lowRightLeg.position);

        //����
        NeckCon = new Controller(neck, Head.position, Forward);

        //ͷ
        HeadCon = new Controller(Head, (leftEye.position + rightEye.position) / 2 - neck.position, Forward);
    }

    private void UpdateAvater()
    {
        //����ת
        Vector3 forward, upward;
        forward = Vector3.Cross(Body[24].transform.position - Body[12].transform.position, Body[23].transform.position - Body[11].transform.position);
        upward = ((Body[11].transform.position + Body[12].transform.position) - (Body[23].transform.position + Body[24].transform.position)) / 2;
        RootCon.UpdateTransform(forward, upward);

        //����ת
        SpineCon.UpdateTransform(forward, upward);

        //��Ťת
        ChestCon.UpdateTransform(forward, upward);

        //�ұ�
        upward = Body[14].transform.position - Body[12].transform.position;
        forward = TriangleForward(Body[14].transform.position, Body[16].transform.position, Body[12].transform.position,1);
        UpRArmCon.UpdateTransform(forward, upward);

        upward = Body[16].transform.position - Body[14].transform.position;
        forward = TriangleForward(Body[14].transform.position, Body[12].transform.position, Body[16].transform.position,1);
        LowRArmCon.UpdateTransform(forward, upward);

        upward = (Body[18].transform.position - Body[16].transform.position) + (Body[20].transform.position - Body[16].transform.position);
        forward = TriangleForward(Body[16].transform.position, Body[20].transform.position, (Body[20].transform.position + Body[18].transform.position) / 2, 1);
        RightHandCon.UpdateTransform(forward, upward);

        //���
        upward = Body[13].transform.position - Body[11].transform.position;
        forward = TriangleForward(Body[13].transform.position, Body[15].transform.position, Body[11].transform.position,1);
        UpLArmCon.UpdateTransform(forward, upward);

        upward = Body[15].transform.position - Body[13].transform.position;
        forward = TriangleForward(Body[13].transform.position, Body[11].transform.position, Body[15].transform.position, 1);
        LowLArmCon.UpdateTransform(forward, upward);

        upward = (Body[17].transform.position - Body[15].transform.position) + (Body[19].transform.position - Body[15].transform.position);
        forward = TriangleForward(Body[15].transform.position, Body[19].transform.position, (Body[19].transform.position + Body[17].transform.position) / 2, 1);
        LeftHandCon.UpdateTransform(forward, upward);

        //����
        upward = Body[26].transform.position - Body[24].transform.position;
        forward = TriangleForward(Body[26].transform.position, Body[28].transform.position, Body[24].transform.position, 2);
        UpRLegCon.UpdateTransform(forward, upward);
        upward = Body[28].transform.position - Body[26].transform.position;
        forward = TriangleForward(Body[26].transform.position, Body[24].transform.position, Body[28].transform.position, 2);
        RKneeCon.UpdateTransform(forward, upward);

        //����
        upward = Body[25].transform.position - Body[23].transform.position;
        forward = TriangleForward(Body[25].transform.position, Body[27].transform.position, Body[23].transform.position, 2);
        UpLLegCon.UpdateTransform(forward, upward);
        upward = Body[27].transform.position - Body[25].transform.position;
        forward = TriangleForward(Body[25].transform.position, Body[23].transform.position, Body[27].transform.position, 2);
        LKneeCon.UpdateTransform(forward, upward);

        //�ҽ���
        upward = Body[32].transform.position - Body[28].transform.position;
        forward = Body[30].transform.position - Body[28].transform.position;
        RightFootCon.UpdateTransform(forward,upward);

        //�����
        upward = Body[31].transform.position - Body[27].transform.position;
        forward = Body[29].transform.position - Body[27].transform.position;
        LeftFootCon.UpdateTransform(forward, upward);

        //����
        forward = RootCon.Forward;
        upward = Vector3.up;
        NeckCon.UpdateTransform(forward, upward);
        neck.rotation = Quaternion.FromToRotation(neck.right, Body[10].transform.position - Body[9].transform.position) * neck.rotation;
        //ͷ
        upward = ((Body[2].transform.position + Body[5].transform.position) - (Body[11].transform.position + Body[12].transform.position)) / 2;
        forward = TriangleNormal(Body[0].transform.position, Body[9].transform.position, Body[10].transform.position) + TriangleNormal(Body[0].transform.position, Body[6].transform.position, Body[3].transform.position);
        neck.rotation = Quaternion.FromToRotation(neck.forward, forward) * neck.rotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitDataPoints();
        InitKeyPoints();
        InitController();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBody();
        UpdateAvater();
        Thread.Sleep(30);
    }
    private void InitDataPoints()
    {
        lines = System.IO.File.ReadLines("Assets/AnimationFile3.txt").ToList();
        m = lines.Count;
    }

    private void InitKeyPoints()
    {
        animator = GetComponent<Animator>();
        //���ɲ���
        root = animator.GetBoneTransform(HumanBodyBones.Hips);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);

        //�ֱ۲���
        lshoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        rshoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        upRightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        lowRightArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        upLeftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        lowLeftArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        wristLeft = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        lmidFinger = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        wristRight = animator.GetBoneTransform(HumanBodyBones.RightHand);
        rmidFinger = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);

        //�Ȳ�����
        upRightLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        lowRightLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);

        upLeftLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        lowLeftLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);

        footLeft = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        leftToe = animator.GetBoneTransform(HumanBodyBones.LeftToes);
        footRight = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        rightToe = animator.GetBoneTransform(HumanBodyBones.RightToes);

        //ͷ��
        neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        Head = animator.GetBoneTransform(HumanBodyBones.Head);

        //�۾�
        leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
        rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);

        Forward = TriangleNormal(root.position, upRightArm.position, upLeftArm.position);
    }


    void UpdateBody()
    {
        //��Ҫ��������ͷʵʱ����ؼ���������ȡ���ⲿ��ע�ͣ�������Ӧ��python�ű�
        /*
        string tmp = udpManager.data;
        if(tmp.Length == 0)
        {
            return;
        }
        string[] points=tmp.Split(',');
        */

        //ͨ����ȡ�ı��Ĺؼ������ݣ��������
        string[] points = lines[counter].Split(',');
        counter++;
        if (counter == lines.Count) counter = 0;

        for (int i = 0; i < 33; i++)
        {
            int k = i * 3;
            float x = float.Parse(points[k]) / 100;
            float y = float.Parse(points[k + 1]) / 100;
            float z = float.Parse(points[k + 2]) / 300;
            Body[i].transform.localPosition = new Vector3(x, y, z);
        }
    }

    private Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    /// <summary>
    /// ȷ���ֱ۲��ֻ��Ȳ��ֵ�������
    /// </summary>
    /// <param name="a">������</param>
    /// <param name="b">����ba</param>
    /// <param name="c">����ca�������������ֱ��ca</param>
    /// <param name="flag">�Ȳ����ֱ�</param>
    /// <returns></returns>
    private Vector3 TriangleForward(Vector3 a,Vector3 b, Vector3 c,int flag)
    {
        Vector3 tmp = TriangleNormal(a, b, c);
        Vector3 dd;

        if (flag == 1)
        {
             dd = Vector3.Cross(c - a, tmp);
        }
        else
        {
            dd = Vector3.Cross(a - c, tmp);
        }
        dd.Normalize();
        return dd;
    }
}
