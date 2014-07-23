using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace ShaderCore.Demo
{
	public class CoreDemo : GameWindow
	{
		Shader shader;

		ShaderContext shaderContext;

		int vao;

		public CoreDemo()
			: base(800, 600, GraphicsMode.Default, "ShaderCore", GameWindowFlags.FixedWindow, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
		{

		}

		public static void Main(string[] args)
		{
			using (CoreDemo demo = new CoreDemo())
			{
				demo.Run(60, 60);
			}
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			InitializeOpenGL();
			InitializeDemo();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.BindVertexArray(this.vao);

			// Select the default shader with Shader.Select.
			// Note that the Shader class caches Select calls and always returns the 
			// same CompiledShader when the same arguments are passed into Select.
			var compiledShader = this.shader.Select();

			// Call glUseProgram for the compiled shader.
			compiledShader.Bind();

			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
			GL.BindVertexArray(0);


			this.SwapBuffers();
		}

		private void InitializeDemo()
		{
			// Initialize a shader context.
			// The context is used for creating shaders,
			// keeping track of the correct OpenGL thread
			// and providing shader input.
			this.shaderContext = new ShaderContext();


			// Create a shader by a given csh file.
			// The csh file is a lua script that contains two global variables:
			// shader
			//   The shader generation object.
			// tag
			//   The tag object given as the second parameter to ShaderContext.Create.
			//   It can be used to generate custom shaders and pass shader generation
			//   variables into the shader script.
			this.shader = this.shaderContext.Create(File.ReadAllText("demoShader.csh"));

			// Shader.Select returns a compiled shader. This shader is generated when needed
			// and can contain overwritten shader fragments or shader fragments for other 
			// shader classes.
			var compiledShader = this.shader.Select();

			// Just print out the uniforms for the selected compiled shader.
			Console.WriteLine("Default shader uniforms:");
			for (int i = 0; i < compiledShader.UniformCount; i++)
			{
				Console.WriteLine("uniform {0} {1};", compiledShader[i].Type, compiledShader[i].Name);
			}

			// Set a uniform value in the CompiledShader.
			// Note that SetValue checks for the right type of the value.
			// Also note that this is a bindless operation. No need to call glUseProgram
			// or CompilerShader.Bind.
			compiledShader["pixelColor"].SetValue(new Vector4(1.0f, 0.0f, 1.0f, 1.0f));
		}

		private void InitializeOpenGL()
		{
			// Just some basic OpenGL initializations to create a triangle.

			GL.ClearColor(0.0f, 0.0f, 0.3f, 1.0f);
			GL.ClearDepth(1.0f);

			this.vao = GL.GenVertexArray();

			float[] g_vertex_buffer_data = {
			   -1.0f, -1.0f, 0.0f,
			   1.0f, -1.0f, 0.0f,
			   0.0f,  1.0f, 0.0f,
			};

			var buffer = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
			GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * g_vertex_buffer_data.Length), g_vertex_buffer_data, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			GL.BindVertexArray(this.vao);
			GL.EnableVertexAttribArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
			GL.VertexAttribPointer(
				0,
				3,
				VertexAttribPointerType.Float,
				false,
				0,
				0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
		}
	}
}
