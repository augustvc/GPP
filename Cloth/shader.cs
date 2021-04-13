using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cloth
{
    class Shader
    {
        int Handle;
        static int BoundHandle = -1;

        public Shader(string vertexPath, string fragmentPath)
        {
            int cnt = 0;
            while (!File.Exists(vertexPath))
            {
                vertexPath = "../" + vertexPath;
                cnt++;
                if (cnt > 100)
                {
                    throw new Exception("Can't find vertex shader file, " + vertexPath);
                }
            }
            cnt = 0;
            while (!File.Exists(fragmentPath))
            {
                fragmentPath = "../" + fragmentPath;
                cnt++;
                if (cnt > 100)
                {
                    throw new Exception("Can't find fragment shader file" + fragmentPath);
                }
            }

            int VertexShader, FragmentShader;

            string vertexShaderSource;
            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            string fragmentShaderSource;
            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentShaderSource);

            GL.CompileShader(VertexShader);

            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != string.Empty)
            {
                Console.WriteLine("Path: " + vertexPath);
                Console.WriteLine("Issue with the vertex shader! \n" + infoLogVert);
            }

            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);
            if (infoLogFrag != string.Empty)
            {
                Console.WriteLine("Path: " + fragmentPath);
                Console.WriteLine("Issue with the fragment shader! \n" + infoLogFrag);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(VertexShader);
            GL.DeleteShader(FragmentShader);
        }

        public void SetMatrix3(string name, Matrix3 mat)
        {
            Use();
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix3(location, true, ref mat);
        }

        public void SetMatrix4(string name, Matrix4 mat)
        {
            Use();
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, true, ref mat);
        }

        public void Use()
        {
            if (BoundHandle == Handle)
                return;
            BoundHandle = Handle;
            GL.UseProgram(Handle);
        }

        public void SetVec4(string name, Vector4 vec4)
        {
            Use();
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform4(location, vec4);
        }

        public void SetVec3(string name, Vector3 vec3)
        {
            Use();
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, vec3);
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform1(location, value);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}