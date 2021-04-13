#version 330 core
layout (location = 0) in vec3 aPosition;

layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

out vec2 TexCoord;
out vec3 Normal;

uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(aPosition, 1.0) * view * projection;
	TexCoord = vec2(aTexCoord.x, aTexCoord.y);

	Normal = aNormal;
}