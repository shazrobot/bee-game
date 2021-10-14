Shader "Custom/BoxBlur"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Size("Blur Radius", Range(0,30)) = 3
        _Directions("Sample Directions", Range(2,16)) = 8
        _Quality("Quality (sample iterations per direction)", Range(1,10)) = 3
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass{
            Tags{ "LightMode" = "Always" }
        }

        Pass
        {
            Tags{ "LightMode" = "Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float4 uvgrab : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                #else
                    float scale = 1.0;
                #endif
                o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.uvgrab.zw = o.vertex.zw;
                o.color = v.color;
                return o;
            }

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Size;
            float _Directions;
            float _Quality;


            fixed4 frag(v2f IN) : SV_Target
            {
                half4 sum = half4(0,0,0,0);
                #define BLURPIXEL(weight,kernel) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(IN.uvgrab.x + _GrabTexture_TexelSize.x * kernel.x*_Size, IN.uvgrab.y + _GrabTexture_TexelSize.y * kernel.y*_Size, IN.uvgrab.z, IN.uvgrab.w))) * weight

                float Pi = 6.28318530718; // Pi*2


                for (float d = 0.0; d < Pi; d += Pi/ _Directions) {
                    for (float i = 1.0 / _Quality; i < 1.0; i+=1.0/ _Quality) {
                        sum += BLURPIXEL(1, float2(cos(d) * i, sin(d) * i));
                    }
                }

                sum /= _Quality * _Directions - (_Directions -1);

                //sum.g = 0;
                //sum.b = 0;
                

                //sum += BLURPIXEL(0.11, float2(0,0));
                //sum += BLURPIXEL(0.11, float2(1, 0));
                //sum += BLURPIXEL(0.11, float2(-1, 0));

                //sum += BLURPIXEL(0.11, float2(0, 1));
                //sum += BLURPIXEL(0.11, float2(1, 1));
                //sum += BLURPIXEL(0.11, float2(-1, 1));

                //sum += BLURPIXEL(0.11, float2(0, -1));
                //sum += BLURPIXEL(0.11, float2(1, -1));
                //sum += BLURPIXEL(0.11, float2(-1, -1));

                //sum.a = tex2D(_MainTex, IN.uvgrab.xy).a;

                sum *= IN.color;

                sum.a = 1;
                //half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * sum;
                //sum += tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * 0 * _Size, i.uvgrab.y + _GrabTexture_TexelSize.y * 0 * _Size, i.uvgrab.z, i.uvgrab.w))) * 1;
                return sum;
            }
            ENDCG
        }
    }
}
