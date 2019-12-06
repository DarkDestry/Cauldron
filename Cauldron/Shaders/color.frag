  
#version 330
in vec3 Diffuse;
out vec4 FragColor;

uniform vec3 LightPosition;
uniform vec3 AmbientMaterial;
uniform vec3 SpecularMaterial;
uniform float Shininess;

void main()
{ 
    vec3 color = Diffuse;
    FragColor = vec4(color, 1.0);
}