Shader "Custom/DirtGradientShader"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        
        _TopColor("Top Color", Color) = (1,1,1,1)
        _BottomColor("Bottom Color", Color) = (1,1,1,1)

        _TopHeight("Top Height", Float) = 20
        _BottomHeight("Bottom Height", Float) = -20

        _FertileTopColor("Fertile Top Color", Color) = (1,1,1,1)
        _FertileBottomColor("Fertile Bottom Color", Color) = (1,1,1,1)
            
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows// noambient

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input
            {
                float2 uv_MainTex;
                float3 worldPos;
                float3 screenPos;

            };

            half _Glossiness;
            half _Metallic;
            float _TopHeight;
            float _BottomHeight;
            float4 _TopColor;
            float4 _BottomColor;
            float4 _FertileTopColor;
            float4 _FertileBottomColor;

            uniform float3 _Position;
            uniform sampler2D _GlobalEffectRT;
            uniform float _OrthographicCamSize;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                half4 dist = IN.worldPos.y;
                half4 weight = (dist - _BottomHeight) / (_TopHeight - _BottomHeight);
                

                half2 bendUV = IN.worldPos.xz;// -_Position.xz;
                bendUV = bendUV / (_OrthographicCamSize * 2);
                bendUV += 0.5;

                half4 fertility = tex2D(_GlobalEffectRT, bendUV);
                
                float distanceSample = 0.001;

                float N = (tex2D(_GlobalEffectRT, bendUV + (0, distanceSample)).g > 0) ? 1 : 0;
                float S = (tex2D(_GlobalEffectRT, bendUV + (0, -distanceSample)).g > 0) ? 1 : 0;
                float W = (tex2D(_GlobalEffectRT, bendUV + (distanceSample, 0)).g > 0) ? 1 : 0;
                float E = (tex2D(_GlobalEffectRT, bendUV + (-distanceSample, 0)).g > 0) ? 1 : 0;

                float NE = (tex2D(_GlobalEffectRT, bendUV + (distanceSample, distanceSample)).g > 0) ? 1 : 0;
                float NW = (tex2D(_GlobalEffectRT, bendUV + (-distanceSample, distanceSample)).g > 0) ? 1 : 0;
                float SE = (tex2D(_GlobalEffectRT, bendUV + (distanceSample, -distanceSample)).g > 0) ? 1 : 0;
                float SW = (tex2D(_GlobalEffectRT, bendUV + (-distanceSample, -distanceSample)).g > 0) ? 1 : 0;

                float fertile = (fertility.g > 0) ? 1 : 0;

                float avgfertility = ((N + S + W + E + NE + NW + SW + SE)/8);

                

                float4 distanceColorDead = lerp(_BottomColor, _TopColor, weight);
                float4 distanceColorFertile = lerp(_FertileBottomColor, _FertileTopColor, weight);;

                half4 distanceColor = lerp(distanceColorDead, distanceColorFertile, avgfertility);
                
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

                o.Albedo = distanceColor.rgb * c.rgb;

                
                //

                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
