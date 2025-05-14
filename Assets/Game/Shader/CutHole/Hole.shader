Shader "HoleShader/Hole"
{
    SubShader
    {
        Tags { "Queue" = "Geometry+1" }  // Juste apr�s les objets opaques
        ColorMask 0                      // Ne dessine pas de couleur
        ZWrite On                        // �crit dans le depth buffer (emp�che le rendu des objets derri�re si pas corrig�)
        
        Stencil 
        {
            Ref 1
            Comp Always
            Pass Replace  // Remplace la valeur du Stencil Buffer par 1
        }

        Pass {}
    }
}

