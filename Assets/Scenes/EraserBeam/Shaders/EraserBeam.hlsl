#ifndef ERASER_BEAM_INCLUDED
#define ERASER_BEAM_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Global
float3 _EraserLightPosWS;
float3 _EraserLightDirWS;
float _EraserLightRange;
float _EraserLightOuterAngle; // degrees
float4 _EraserLightColor;     // rgba
float _EraserLightIntensity;

struct EraserLightData
{
    float3 direction;
    float distanceAtten;
    float angleAtten;
    float4 color;
};

EraserLightData GetEraserLight(float3 positionWS)
{
    EraserLightData data;

    float3 toLight = _EraserLightPosWS - positionWS;
    float distance = length(toLight);
    float3 lightDir = normalize(toLight);
    float3 beamDir = normalize(_EraserLightDirWS);

    float distAtten = saturate(1.0 - distance / _EraserLightRange);
    float angleCos = dot(lightDir, beamDir);
    float outerCos = cos(radians(_EraserLightOuterAngle * 0.5));
    float angleAtten = saturate((angleCos - outerCos) * 20.0); // Falloff 조정

    data.direction = lightDir;
    data.distanceAtten = distAtten;
    data.angleAtten = angleAtten;
    data.color = _EraserLightColor * _EraserLightIntensity;

    return data;
}

float rand(float t)
{
    return frac(sin(dot(float2(t, t), float2(12.9898, 78.233))) * 43758.5453);
}

float3 SampleStylizedTrail(float2 uv , float time)
{
    float2 uv1 = uv - float2(0.8, 0.4);
    float r = length(uv1) * 111.0;

    float t = ceil(r);
    float angle = atan2(uv1.y, uv1.x);
    float a = frac(angle / 3.14159265 + time * rand(t) * 0.1 + t * 0.1);

    float ang = rand(t);
    float c = smoothstep(ang, ang - 1.5, a * 5.0);

    float3 col = float3(0.3, 0.3, 0.5) * 3.0;
    float rr = length(uv - float2(0.6, 1.4)) - 0.8;
    float3 coll = float3(0.0, rr * 0.1, rr * 0.24);

    return lerp(coll, col * rand(t), c * step(0.1, r / 111.0));
}

#endif // ERASER_BEAM_INCLUDED
