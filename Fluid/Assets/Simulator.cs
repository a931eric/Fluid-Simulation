using UnityEngine;

public class Simulator : MonoBehaviour
{
    public int num_iter;
    public Vector3Int size;
    public RenderTexture renderTexture;
    Vector2Int imageSize;

    
    public ComputeShader shader;
    public int GS=8;

    public float scale,q,hx,ht,k,damp;
    public Vector3 g;
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
        r1 = new ComputeBuffer(count, sizeof(float));
        rData = new float[count];
        for(int x = 0; x < size.x; x++) 
            for (int y = 0; y < size.y; y++) 
                for (int z = 0; z < size.z; z++) 
                { rData[x+y*size.x+z*size.x*size.y] =1; }
        r.SetData(rData);
        r1.SetData(rData);

        u = new ComputeBuffer(count, sizeof(float) * 3);
        u1 = new ComputeBuffer(count, sizeof(float) * 3);
        uData = new Vector3[count];
        for (int i = 0; i < count; i++) { uData[i] = new Vector3(); }
        u.SetData(uData);
        u1.SetData(uData);

        renderTexture = new RenderTexture(renderTexture);
        renderTexture.enableRandomWrite = true;
        renderTexture.wrapMode = TextureWrapMode.Repeat;
        renderTexture.Create();
        imageSize = new Vector2Int(renderTexture.width, renderTexture.height);

        shader.SetBuffer(R, "r", r);
        shader.SetBuffer(R, "r1", r1);
        shader.SetBuffer(R, "u", u);

        shader.SetBuffer(U, "r1", r1);
        shader.SetBuffer(U, "u", u);
        shader.SetBuffer(U, "u1", u1);
        
        shader.SetBuffer(Upd, "r", r);
        shader.SetBuffer(Upd, "r1", r1);
        shader.SetBuffer(Upd, "u", u);
        shader.SetBuffer(Upd, "u1", u1);

        shader.SetBuffer(Out, "r", r);
        shader.SetBuffer(Out, "u", u);
        shader.SetTexture(Out, "o", renderTexture);
    }

    void Update()
    {
        shader.SetFloat("hx", hx);
        shader.SetFloat("ht", ht);
        shader.SetFloat("k", k);
        shader.SetFloat("damp", damp);
        shader.SetFloats("g", new float[] { g.x, g.y, g.z });
        for (int i = 0; i < num_iter; i++)
        {
            shader.Dispatch(R, size.x / GS, size.y / GS, size.z / GS);
            shader.Dispatch(U, size.x / GS, size.y / GS, size.z / GS);
            shader.Dispatch(Upd, size.x / GS, size.y / GS, size.z / GS);
        }

        shader.SetFloat("scale", scale);
        Vector3 camPos = Camera.main.transform.position;
        shader.SetFloats("camPos", new float[] { camPos.x, camPos.y, camPos.z });
        Matrix4x4 camRot = Matrix4x4.Rotate(Camera.main.transform.rotation);
        shader.SetMatrix("camRot", camRot);
        shader.SetFloats("color", new float[] { color.r, color.g, color.b, color.a });
        shader.SetFloat("d", d);
        shader.SetFloat("q",q);

        shader.Dispatch(Out, imageSize.x / GS, imageSize.y / GS,1);
        
        quad.material.SetTexture("_MainTex", renderTexture);
    }
    void OnGUI()
    {
        Graphics.DrawTexture(new Rect((Screen.width- imageSize.x)/2, (Screen.height - imageSize.y) / 2,imageSize.x, imageSize.y), renderTexture);
    }
    
}
