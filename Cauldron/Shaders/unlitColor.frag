#version 330
in vec3 Diffuse;
out vec4 FragColor;

void main()
{
    FragColor = vec4(Diffuse, 1.0);
}