#version 330 core
out vec4 outputColor;
in vec3 Normal;
in vec2 TexCoord;

uniform sampler2D texture1;

void main()
{
	float light = max(dot(vec3(0, 0.707, 0.707), Normal) * 0.6 + 0.4, 0.5);
	
	outputColor = texture(texture1, TexCoord) * light;
}