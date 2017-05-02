using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
	const int sizeBit = 8;
	const int sizeShift = sizeBit - 1;
	const int sizeDim = 1 << sizeShift;
	const int size = sizeDim * sizeDim;

	public ComputeShader shader;
	int particleKernel, clearKernel, length;
	ComputeBuffer particleBuffer, gridBuffer;

	struct Particle
	{
		public Vector2 position;
		public Vector2 speed;
		public Vector2 acceleration;
		public Vector4 color;
	};
	
	void Start()
	{
		Mesh mesh = new Mesh();
		int sizeOver64 = size >> 6;
        Vector3[] vertices = new Vector3[sizeOver64];
		for (int i = 0; i < sizeOver64; i++)
				vertices[i] = new Vector3(i << 6, 0);
		mesh.vertices = vertices;
		int[] indices = new int[sizeOver64];
		for (int i = 0; i < sizeOver64; i++)
			indices[i] = i;
		mesh.SetIndices(indices, MeshTopology.Points, 0);
		GetComponent<MeshFilter>().mesh = mesh;

		Particle[] data = new Particle[size];
		Vector3[] grid = new Vector3[size];
		for (int i = 0; i < sizeDim; i++)
			for (int j = 0; j < sizeDim; j++)
				try
				{
					data[i + (j << sizeShift)] = new Particle { position = new Vector2(/*(i - (sizeDim >> 1)) / 64F + Random.Range(-0.01F, 0.01F), (j - (sizeDim >> 1)) / 64F + Random.Range(-0.01F, 0.01F)*/Random.Range(-4F, 4F), Random.Range(-4F, 4F)), speed = Vector3.zero, acceleration = Vector3.zero, color = Vector4.zero };
					grid[i + (j << sizeShift)] = Vector3.zero;
				}
				catch(System.Exception ex)
				{
					Debug.Log(size);
					Debug.Log(i + (j << sizeShift));
					Debug.Log(new Vector2(i, j));
					Debug.Log(ex);
					return;
				}
		length = data.Length;

		particleBuffer = new ComputeBuffer(length, 4 * 2 * 3 + 4 * 4);
		gridBuffer = new ComputeBuffer(length, 4 * 2);

		particleBuffer.SetData(data);
		gridBuffer.SetData(grid);

		particleKernel = shader.FindKernel("ParticleTest");
		clearKernel = shader.FindKernel("ClearVectorBuffer");
        shader.SetBuffer(particleKernel, "particleBuffer", particleBuffer);
		shader.SetBuffer(particleKernel, "gridBuffer", gridBuffer);
		shader.SetBuffer(clearKernel, "buffer", gridBuffer);
		GetComponent<Renderer>().material.SetBuffer("dataBuffer", particleBuffer);
	}

	void Update()
	{
		shader.SetFloat("deltaTime", Time.deltaTime);
		shader.Dispatch(clearKernel, size >> 6, 1, 1); //64, 64, 64);
		shader.Dispatch(particleKernel, size >> 6, 1, 1); //64, 64, 64);
	}
}
