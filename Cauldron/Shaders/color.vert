#version 330
layout(location = 0) in vec4 Position;
layout(location = 1) in vec4 Normal;

uniform mat4 Projection;
uniform mat4 Model;
uniform mat4 View;
uniform mat3 NormalMatrix;
uniform vec3 DiffuseMaterial;

out vec3 Diffuse;
out vec3 N;

void main()
{
    gl_Position = Projection * View * Model * Position;
    Diffuse = DiffuseMaterial;
	N = Normal.xyz;
}