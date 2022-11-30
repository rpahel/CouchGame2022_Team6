using UnityEngine;

namespace CustomMaths
{
    public static class CustomPhysics
    {
        /// <summary>
        /// Fais une sorte de raycast en forme de carré. Oui ça existe déjà mais j'arrivais pas à le faire fonctionner donc voilà.
        /// </summary>
        /// <param name="origin">La position du centre du carré dans le monde.</param>
        /// <param name="size">La longueur d'un coté du carré.</param>
        /// <param name="drawDebug">Est-ce qu'on dessine le carré en mode debug ?</param>
        /// <returns>La liste de tous les RaycastHit2D générés.</returns>
        public static RaycastHit2D[] SquareCast(Vector2 origin, float size, bool drawDebug = false)
        {
            Vector2[] localPositions = new Vector2[8];
            Vector2[] worldPositions = new Vector2[8];
            RaycastHit2D[] hits = new RaycastHit2D[8];

            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0: localPositions[i] = new Vector2(0, 1); break;
                    case 1: localPositions[i] = new Vector2(1, 1); break;
                    case 2: localPositions[i] = new Vector2(1, 0); break;
                    case 3: localPositions[i] = new Vector2(1, -1); break;
                    case 4: localPositions[i] = new Vector2(0, -1); break;
                    case 5: localPositions[i] = new Vector2(-1, -1); break;
                    case 6: localPositions[i] = new Vector2(-1, 0); break;
                    case 7: localPositions[i] = new Vector2(-1, 1); break;
                }

                worldPositions[i] = origin + .5f * size * localPositions[i];
            }

            for (int i = 0; i < 8; i++)
            {
                hits[i] = Physics2D.Linecast(worldPositions[i], worldPositions[(i + 4) % 8]);

#if UNITY_EDITOR
                {
                    if (drawDebug)
                        Debug.DrawLine(worldPositions[i], worldPositions[(i + 4) % 8], Color.red, 5f);
                }
#endif
            }

            return hits;
        }
    }

    public static class CustomVectors
    {
        /// <summary>
        /// Arrondit la normale donnée à la direction cardinale la plus proche (N, NE, E, SE, S, SW, W ou NW)
        /// </summary>
        /// <param name="normalToApproximate">La normale à arrondir.</param>
        public static Vector2 ApproxNormal(Vector2 normalToApproximate)
        {
            normalToApproximate.Normalize();
            const float cos30 = 0.866f;

            if (normalToApproximate.y > cos30 && (normalToApproximate.x > -.5f && normalToApproximate.x < .5f))
                return Vector2.up;            // N
            else if ((normalToApproximate.x > .5f && normalToApproximate.x < cos30) && (normalToApproximate.y > .5f && normalToApproximate.y < cos30))
                return Vector2.one;           // NE
            else if (normalToApproximate.x > cos30 && (normalToApproximate.y > -.5f && normalToApproximate.y < .5f))
                return Vector2.right;         // E
            else if ((normalToApproximate.x > .5f && normalToApproximate.x < cos30) && (normalToApproximate.y < -.5f && normalToApproximate.y > -cos30))
                return new Vector2(1, -1);    // SE
            else if (normalToApproximate.y < -cos30 && (normalToApproximate.x > -.5f && normalToApproximate.x < .5f))
                return Vector2.down;          // S
            else if ((normalToApproximate.x > -cos30 && normalToApproximate.x < -.5f) && (normalToApproximate.y < -.5f && normalToApproximate.y > -cos30))
                return -Vector2.one;           // SW
            else if (normalToApproximate.x < -cos30 && (normalToApproximate.y > -.5f && normalToApproximate.y < .5f))
                return Vector2.left;          // W
            else if ((normalToApproximate.x > -cos30 && normalToApproximate.x < -.5f) && (normalToApproximate.y > .5f && normalToApproximate.y < cos30))
                return new Vector2(-1, 1);    // NW

            Debug.Log("Tu ne devrais pas voir ça.");
            return Vector2.up;
        }

        /// <summary>
        /// Retourne l'axe (up, down, left, right) le plus proche de la direction donnée.
        /// </summary>
        /// <param name="direction">La direction que l'on souhaite arrondir à l'axe le plus proche.</param>
        public static Vector2 ClosestAxis(Vector2 direction)
        {
            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                if (direction.y > 0) return Vector2.up;
                else return Vector2.down;
            }
            else
            {
                if (direction.x >= 0) return Vector2.right;
                else return Vector2.left;
            }
        }
    }

    public static class CustomDebugs
    {
        /// <summary>
        /// Dessine une petite WireSphere comme Gizmos.DrawWireSphere mais en Debug.
        /// </summary>
        /// <param name="center">Centre de la sphere dans le monde.</param>
        /// <param name="radius">Rayon de la sphere.</param>
        /// <param name="color"></param>
        /// <param name="duration">Durée de vie de la sphere.</param>
        /// <param name="quality">Qualité de la sphere. Un plus grand nombre veut dire plus de traits.</param>
        public static void DrawWireSphere(Vector3 center, float radius, Color color, float duration, int quality = 3)
        {
            quality = Mathf.Clamp(quality, 1, 10);

            int segments = quality << 2;
            int subdivisions = quality << 3;
            int halfSegments = segments >> 1;
            float strideAngle = 360F / subdivisions;
            float segmentStride = 180F / segments;

            Vector3 first;
            Vector3 next;
            for (int i = 0; i < segments; i++)
            {
                first = (Vector3.forward * radius);
                first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.right) * first;

                for (int j = 0; j < subdivisions; j++)
                {
                    next = Quaternion.AngleAxis(strideAngle, Vector3.up) * first;
                    UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
                    first = next;
                }
            }

            Vector3 axis;
            for (int i = 0; i < segments; i++)
            {
                first = (Vector3.forward * radius);
                first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.up) * first;
                axis = Quaternion.AngleAxis(90F, Vector3.up) * first;

                for (int j = 0; j < subdivisions; j++)
                {
                    next = Quaternion.AngleAxis(strideAngle, axis) * first;
                    UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
                    first = next;
                }
            }
        }
    }
}
