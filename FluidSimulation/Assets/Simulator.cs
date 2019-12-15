using UnityEngine;

public class Simulator : MonoBehaviour
{
    public int num_iter;
    public Vector3Int size;
    public RenderTexture renderTexture;
    public Vector2 imageSize;

    public int GS=8;
    
    public ComputeShader shader;
    public ComputeBuffer r,r1,u,u1;
    public float[] rData;
    public Vector3[] uData;
    int R = 0, U = 1, Upd = 2, Out = 3;//kernel ids
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

        shader.SetBuffer(R, "r", r);
        shader.SetBuffer(R, "u", u);
        shader.SetBuffer(U, "u", u);
        shader.SetBuffer(Upd, "r", r);
        shader.SetBuffer(Upd, "u", u);
        shader.SetBuffer(Out, "r", r);
        shader.SetBuffer(Out, "u", u);

    }
    void Update()
    {
        for(int i = 0; i < num_iter; i++)
        {
            shader.Dispatch(R, size.x / GS, size.y / GS, size.z / GS);
            shader.Dispatch(U, size.x / GS, size.y / GS, size.z / GS);
            shader.Dispatch(Upd, size.x / GS, size.y / GS, size.z / GS);
        }
        shader.Dispatch(Out,imageSize/GS)
    }
}
