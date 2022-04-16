// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*Shader "Custom/ToonTerrain2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}*/

Shader "Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass" {
    Properties{
        _Control("Control (RGBA)", 2D) = "red" {}
        _Splat3("Layer 3 (A)", 2D) = "white" {}
        _Splat2("Layer 2 (B)", 2D) = "white" {}
        _Splat1("Layer 1 (G)", 2D) = "white" {}
        _Splat0("Layer 0 (R)", 2D) = "white" {}
        // used in fallback on old cards
        _MainTex("BaseMap (RGB)", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Outline("Outline width", Range(.002, 0.03)) = .003
        _Color("Main Color", Color) = (1,1,1,1)
        _ToonShade("ToonShader Cubemap(RGB)", CUBE) = "" { Texgen CubeNormal }
    }

        CGINCLUDE
#include "UnityCG.cginc"

        struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f {
        float4 pos : POSITION;
        float4 color : COLOR;
    };

    uniform float _Outline;
    uniform float4 _OutlineColor;

    v2f vert(appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);

        float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
        float2 offset = TransformViewToProjection(norm.xy);

        o.pos.xy += offset * o.pos.z * _Outline;
        o.color = _OutlineColor;
        return o;
    }
    ENDCG

        SubShader{
            Tags {
                "SplatCount" = "4"
                "Queue" = "Geometry-100"
                "RenderType" = "Opaque"
            }
                Pass {
                    Name "OUTLINE"
                    Tags { "LightMode" = "Always" }
                    Cull Front
                    ZWrite On
                    ColorMask RGB
                    Blend SrcAlpha OneMinusSrcAlpha

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    half4 frag(v2f i) :COLOR { return i.color; }
                    ENDCG
                }
        CGPROGRAM
        #pragma surface surf Lambert
        struct Input {
            float2 uv_Control : TEXCOORD0;
            float2 uv_Splat0 : TEXCOORD1;
            float2 uv_Splat1 : TEXCOORD2;
            float2 uv_Splat2 : TEXCOORD3;
            float2 uv_Splat3 : TEXCOORD4;
        };

        sampler2D _Control;
        sampler2D _Splat0,_Splat1,_Splat2,_Splat3;

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 splat_control = tex2D(_Control, IN.uv_Control);
            fixed3 col;
            col = splat_control.r * tex2D(_Splat0, IN.uv_Splat0).rgb;
            col += splat_control.g * tex2D(_Splat1, IN.uv_Splat1).rgb;
            col += splat_control.b * tex2D(_Splat2, IN.uv_Splat2).rgb;
            col += splat_control.a * tex2D(_Splat3, IN.uv_Splat3).rgb;
            o.Albedo = col;
            o.Alpha = 0.0;
        }
        ENDCG
    }

        SubShader{
        Tags { "RenderType" = "Opaque" }
        UsePass "Toon/Basic/BASE"
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            // Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
            #pragma exclude_renderers gles
                        #pragma vertex vert
                        #pragma exclude_renderers shaderonly
                        ENDCG
                        SetTexture[_MainTex] { combine primary }
                    }
        }


            // Fallback to Diffuse
            Fallback "Diffuse"
}
