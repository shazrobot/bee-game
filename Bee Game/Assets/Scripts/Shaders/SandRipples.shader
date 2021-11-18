Shader "Custom/SandRipples"
{
    Properties
    {
        _SandColour ("Color", Color) = (1,1,1,1)
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
        //#pragma surface surf Standard fullforwardshadows
        #pragma surface surf Journey fullforwardshadows
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        float4 _SandColour;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
        float4 LightingJourney (SurfaceOutput s, fixed3 viewDir, UnityGI gi)
        {
            float3 L = gi.light.dir;
            float3 N = s.Normal;

            float3 diffuseColor = DiffuseColor(N,L);

            //float3 diffuseColor = DiffuseColor    ();
            //float3 rimColor     = RimLighting     ();
            //float3 oceanColor   = OceanSpecular   ();
            //float3 glitterColor = GlitterSpecular ();
            //float3 specularColor = saturate(max(rimColor, oceanColor));
            //float3 color = diffuseColor + specularColor + glitterColor;

            return float4(color * s.Albedo, 1);
        }

        /*
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
        */
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _SandColour;
            o.Alpha = 1;
            float3 N = float3(0, 0, 1);
            //N = RipplesNormal(N);
            //N = SandNormal(N);
            o.Normal = N;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
