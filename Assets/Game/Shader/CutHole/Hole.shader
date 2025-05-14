Shader "HoleShader/Hole"
{
    SubShader
    {
        Tags { "Queue" = "Geometry+1" }  // Juste après les objets opaques
        ColorMask 0                      // Ne dessine pas de couleur
        ZWrite On                        // Écrit dans le depth buffer (empêche le rendu des objets derrière si pas corrigé)
        
        Stencil 
        {
            Ref 1
            Comp Always
            Pass Replace  // Remplace la valeur du Stencil Buffer par 1
        }

        Pass {}
    }
}

