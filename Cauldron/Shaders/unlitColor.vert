#version 330
layout(location = 0) in vec4 Position;

uniform mat4 Projection;
uniform mat4 Model;
uniform mat4 View;
uniform vec3 DiffuseMaterial;

out vec3 Diffuse;

void main()
{
    gl_Position = Projection * View * Model * Position;
    Diffuse = DiffuseMaterial;
}