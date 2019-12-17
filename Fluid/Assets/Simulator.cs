using UnityEngine;

public class Simulator : MonoBehaviour
{
    public int num_iter;
    public Vector3Int size;
    public RenderTexture renderTexture;
    public Vector2Int imageSize;

    
    public ComputeShader shader;
    public int GS=8;

    public float scale;

    public float d;// d unit: pixel
    public Color color;

    ComputeBuffer r,r1,u,u1;
    float[] rData;
    Vector3[] uData;
    public MeshRenderer quad;
    int R = 0, U = 1, Upd = 2, Out = 3;//kernel ids

    private void Start()
    {
        SetupComputeShader();
        R = shader.FindKernel("R");
        U = shader.FindKernel("U");
        Upd = shader.FindKernel("Update");
        Out = shader.FindKernel("Output");
    }

    void SetupComputeShader()
    {
     
        if (r != null) r.Release();
        if (r1 != null) r1.Release();
        if (u != null) u.Release();
        if (u1 != null) u1.Release();
        int count = size.x * size.y * size.z;

        r = new ComputeBuffer(count, sizeof(float));
        rData = new float[count];
        for(int i = 0; i < count; i++) { rData[i] = 0.1f; }
        r.SetData(rData);

        u = new ComputeBuffer(count, sizeof(float) * 3);
        uData = new Vector3[count];
        for (int i = 0; i < count; i++) { uData[i] = new Vector3(); }
        u.SetData(uData);

        renderTexture = new RenderTexture(renderTexture);
        renderTexture.enableRandomWrite = true;
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.Create();

        shader.SetBuffer(R, "r", r);
        shader.SetBuffer(R, "u", u);
        shader.SetBuffer(U, "u", u);
        shader.SetBuffer(Upd, "r", r);
        shader.SetBuffer(Upd, "u", u);
        shader.SetBuffer(Out, "r", r);
        shader.SetBuffer(Out, "u", u);
        shader.SetTexture(Out, "o", renderTexture);
    }

    void Update()
    {

        for(int i = 0; i < num_iter; i++)
        {
            shader.Dispatch(R, size.x / GS, size.y / GS, size.z / GS);
            shader.Dispatch(U, size.x / GS, size.y / GS, size.z / GS);
            shader.Dispatch(Upd, size.x / GS, size.y / GS, size.z / GS);
        }

        shader.SetFloat("scale", scale);
        Vector3 camPos = Camera.main.transform.position;
        shader.SetFloats("camPos", new float[] { camPos.x, camPos.y, camPos.z });
        Matrix4x4 camRot = Matrix4x4.Rotate(Camera.main.transform.rotation);
        shader.SetFloats("camRot", new float[] { camRot.m00, camRot.m01, camRot.m02 , camRot.m10, camRot.m11, camRot.m12 , camRot.m20, camRot.m21, camRot.m22 });
        shader.SetFloats("color", new float[] { color.r, color.g, color.b, color.a });
        shader.SetFloat("d", d);

        shader.Dispatch(Out, imageSize.x / GS, imageSize.y / GS,1);
        
        //quad.material.SetTexture("_BaseMap", renderTexture);
        
    }
    void OnGUI()
    {
        Graphics.DrawTexture(new Rect(50, 50 , 500, 500), renderTexture);
    }
    
}
