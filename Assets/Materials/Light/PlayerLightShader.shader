// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "PlayerLightShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
            

            struct a2v
            {
                fixed4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vtof
            {
                fixed4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            vtof SpriteVert (a2v v)
            {
                vtof o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            fixed4 SpriteFrag (vtof i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv); //将纹理贴在材质上
                //进行颜色(254,254,160)的叠加算法，（叠加算法：C = （A * B）/128，A, B, C均为RGB数值）
                //step(a, x)	Returns (a <= x) ? 1 : 0
                bool flag = step(0.03, col.rgb); //黑色不变色
                bool flag2 = step(i.uv.x, 0.5384); //只变左半部分
                col.rg += ((col.rg*254)/128-col.rg)*flag*flag2;
                col.b += ((col.b*160)/128-col.b)*flag*flag2;
                return col;
            }
        ENDCG
        }
    }
}