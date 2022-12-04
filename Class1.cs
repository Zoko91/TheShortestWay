using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ProjetIA2022
{
    public class Node2 : GenericNode 
    {
        public int x;
        public int y;

        // Méthodes abstrates, donc à surcharger obligatoirement avec override dans une classe fille
        public override bool IsEqual(GenericNode N2)
        {
            Node2 N2bis = (Node2)N2;

            return (x == N2bis.x) && (y == N2bis.y);
        }

        public override double GetArcCost(GenericNode N2)
        {
            // Ici, N2 ne peut être qu'1 des 8 voisins, inutile de le vérifier
            Node2 N2bis = (Node2)N2;
            double dist = Math.Sqrt((N2bis.x-x)*(N2bis.x-x)+(N2bis.y-y)*(N2bis.y-y));
            if (Form1.matrice[x, y] == -1)
                // On triple le coût car on est dans un marécage
                dist = dist*3;
            return dist;
        }

        public override bool EndState()
        {
            return (x == Form1.xfinal) && (y == Form1.yfinal);
        }

        public override List<GenericNode> GetListSucc()
        {
            List<GenericNode> lsucc = new List<GenericNode>();

            for (int dx=-1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if ((x + dx >= 0) && (x + dx < Form1.nbcolonnes)
                                      && (y + dy >= 0) && (y + dy < Form1.nblignes) && ((dx != 0) || (dy != 0)))
                        if (Form1.matrice[x + dx, y + dy] > -2)
                        {
                            Node2 newnode2 = new Node2();
                            newnode2.x = x + dx;
                            newnode2.y = y + dy;
                            lsucc.Add(newnode2);
                        }
                }

            }
            return lsucc;
        }

        public double Euclidienne(int xInitial, int yInitial,int xFinal, int yFinal)
        {
            // Calcul de la distance euclidienne entre 2 points
            // calcul de la distance sur la diagonale + rajout des cases en déplacement horizontal ou vertical pour rejoindre le point final
            double distanceEuclidienne = 0;
            int differenceX = Math.Abs(xFinal - xInitial);
            int differenceY = Math.Abs(yFinal - yInitial);
            // La difference la plus grande nous indique si le carré est plus grand en largeur ou longeur 
            int casesSupplementaires = Math.Abs(differenceX - differenceY);
            
            if (differenceX > differenceY)
            {
                distanceEuclidienne = differenceY * Math.Sqrt(2) + casesSupplementaires;
            }
            else
            {
                distanceEuclidienne = differenceX * Math.Sqrt(2) + casesSupplementaires;
            }
            return distanceEuclidienne;
        }

        public override double CalculeHCost()
        {   
            // Récupération des points finaux
            int xFinal = Form1.xfinal;
            int yFinal = Form1.yfinal;
            double distanceEucli = 0;

            // Heuristique de l'environnement 2
            // --------------------------------------
            if (Form1.matrice[10, 0] == -2) // ENV 2
            // --------------------------------------
            {
                if (x < 10) 
                    // Partie gauche
                {
                    if (xFinal <10) 
                        // point d'arrivée dans la même zone que le point de départ
                    {
                        distanceEucli = Euclidienne(x, y, xFinal, yFinal);
                    }
                    else // point de départ à gauche et point d'arrivée à droite
                    {
                        int xFIntermediaire = 10;
                        int yFIntermediaire = 8;
                        distanceEucli = Euclidienne(x,y, xFIntermediaire, yFIntermediaire) + Euclidienne(xFIntermediaire, yFIntermediaire, xFinal, yFinal); 
                    }
                }
                else if (x > 10)
                {
                    if (Form1.xfinal >= 10) // point d'arrivée dans la même zone que le point de départ
                    {
                        distanceEucli = Euclidienne(x,y,xFinal, yFinal);
                    }
                    else // point de départ à gauche et point d'arrivée à droite
                    {

                        // calcul du trajet jusqu'au centre de l'environnement, dans le goulot
                        
                        int xFIntermediaire = 10;
                        int yFIntermediaire = 8;
                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaire, yFIntermediaire) 
                            + Euclidienne(xFIntermediaire,yFIntermediaire, xFinal,yFinal);

                    }
                }
            }
            // Heuristique de l'environnement 3
            // --------------------------------------
            else if (Form1.matrice[10, 0] == 0 && Form1.matrice[10, 1] == -2) // ENV 3
            // --------------------------------------
            {
                if (x > 2 && x <= 9 && y >= 4 && y <= 9) // Zone 1 : Dans le cercle
                {
                    int xFIntermediaireSortieCercle = 2;
                    int yFIntermediaireSortieCercle = 6;

                    if (xFinal >= 0 && xFinal <=9 && yFinal >=4 && yFinal <= 9) //deux points dans le cercle
                    {
                        distanceEucli = Euclidienne(x,y,xFinal, yFinal);
                    }
                    else if(xFinal >= 0 && xFinal <=10 && yFinal >= 11) //point d'arrivée en bas à gauche
                    {
                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaireSortieCercle, yFIntermediaireSortieCercle) 
                            + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle,xFinal, yFinal);
                    }
                    else if(xFinal >= 0 && xFinal <=10 && yFinal <= 2) //point d'arrivée en haut à gauche
                    {
                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaireSortieCercle, yFIntermediaireSortieCercle) 
                            + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle, xFinal, yFinal);
                    }
                    else if(xFinal >= 11 && yFinal <= 9) //point d'arrivée dans le marécage
                    {
                        int xFIntermediaire = 11;
                        int yFIntermediaire = 0 ;

                        distanceEucli = 
                            Euclidienne(x,y, xFIntermediaireSortieCercle, yFIntermediaireSortieCercle) 
                            + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle, xFIntermediaire, yFIntermediaire) 
                            + 3*Euclidienne(xFIntermediaire, yFIntermediaire, xFinal, yFinal);
                    }
                    else if(xFinal >= 11 && yFinal >= 10) //point d'arrivée en bas à droite
                    {
                        int xFIntermediaire = 11;
                        int yFIntermediaire = 0 ;

                        int xFSortieMarecage = 11;
                        int yFSortieMarecage = 10 ;

                        distanceEucli = Euclidienne(x, y, xFIntermediaireSortieCercle, yFIntermediaireSortieCercle)
                                        + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle,xFIntermediaire, yFIntermediaire) 
                                        + 3*Euclidienne(xFIntermediaire, yFIntermediaire, xFSortieMarecage,  yFSortieMarecage) 
                                        + Euclidienne(xFSortieMarecage, yFSortieMarecage, xFinal, yFinal);
                    }

                    
                }
                else if (x <= 9 && y >= 11) // Zone 2 :  En bas à gauche
                {
                    if ((xFinal >= 0 && xFinal <=10 && yFinal >= 11 && yFinal <= 19) || (xFinal >= 8 && xFinal <=9 && yFinal >= 9 && yFinal <= 10) ) // deux points dans la zone 2 || petit triangle
                    {
                        distanceEucli = Euclidienne(x,y,xFinal, yFinal);
                    }
                    else if(xFinal > 2 && xFinal <= 9 && yFinal >= 4 && yFinal <= 9) // point d'arrivée dans le cercle
                    {
                        int xFIntermediaireSortieCercle = 2;
                        int yFIntermediaireSortieCercle = 6;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaireSortieCercle, yFIntermediaireSortieCercle) 
                            + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle,xFinal, yFinal);
                    }
                    else if(xFinal >= 0 && xFinal <=10 && yFinal <= 2) // point d'arrivée en haut à gauche 
                    {
                        int xFIntermediaire = 1;
                        int yFIntermediaire = 9;

                        int xFIntermediaire2 = 1;
                        int yFIntermediaire2 = 4;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaire, yFIntermediaire) 
                            + Euclidienne(xFIntermediaire, yFIntermediaire,xFIntermediaire2, yFIntermediaire2) 
                            + Euclidienne(xFIntermediaire2, yFIntermediaire2,xFinal, yFinal);
                    }
                    else if(xFinal >= 11 && yFinal <= 9) // point d'arrivée dans le marécage 
                    {
                        int xFIntermediaire = 1;
                        int yFIntermediaire = 9;

                        int xFIntermediaire2 = 1;
                        int yFIntermediaire2 = 4;

                        int xFIntermediaire3 = 11;
                        int yFIntermediaire3 = 0;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaire, yFIntermediaire) 
                            + Euclidienne(xFIntermediaire, yFIntermediaire,xFIntermediaire2, yFIntermediaire2) 
                            + Euclidienne(xFIntermediaire2, yFIntermediaire2,xFIntermediaire3, yFIntermediaire3) 
                            +3* Euclidienne(xFIntermediaire3, yFIntermediaire3,xFinal, yFinal);
                    }
                    else if(xFinal >= 11 && yFinal >= 10) //point d'arrivée en bas à droite 
                    {
                        int xFIntermediaire = 1;
                        int yFIntermediaire = 9;

                        int xFIntermediaire2 = 1;
                        int yFIntermediaire2 = 4;

                        int xFIntermediaire3 = 11;
                        int yFIntermediaire3 = 0;

                        int xFSortieMarecage = 11;
                        int yFSortieMarecage = 10 ;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaire, yFIntermediaire) 
                            + Euclidienne(xFIntermediaire, yFIntermediaire,xFIntermediaire2, yFIntermediaire2) 
                            + Euclidienne(xFIntermediaire2, yFIntermediaire2,xFIntermediaire3, yFIntermediaire3) 
                            + 3*Euclidienne(xFIntermediaire3, yFIntermediaire3,xFSortieMarecage, yFSortieMarecage) 
                            + Euclidienne(xFSortieMarecage, yFSortieMarecage,xFinal, yFinal);
                    }
                    
                }
                else if (x <= 10 && y <= 2) // Zone 3 : En haut à gauche
                {
                    if ((xFinal >= 0 && xFinal <=10 && yFinal >= 0 && yFinal <= 2) || (xFinal >= 8 && xFinal <=10 && yFinal >= 3 && yFinal <= 4) ) // deux points dans la zone 3 || petit triangle
                    {
                        distanceEucli = Euclidienne(x,y,xFinal, yFinal);
                    }
                    else if(xFinal > 2 && xFinal <= 9 && yFinal >= 4 && yFinal <= 9) // point d'arrivée dans le cercle
                    {
                        int xFIntermediaireSortieCercle = 2;
                        int yFIntermediaireSortieCercle = 6;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaireSortieCercle, yFIntermediaireSortieCercle) 
                            + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle, xFinal, yFinal);
                    }
                    else if(xFinal >= 0 && xFinal <=10 && yFinal >= 11) // point d'arrivée en bas à gauche 
                    {
                        int xFIntermediaire = 1;
                        int yFIntermediaire = 4;
                        
                        int xFIntermediaire2 = 1;
                        int yFIntermediaire2 = 9;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaire, yFIntermediaire) 
                            + Euclidienne(xFIntermediaire, yFIntermediaire,xFIntermediaire2, yFIntermediaire2) 
                            + Euclidienne(xFIntermediaire2, yFIntermediaire2,xFinal, yFinal);
                    }
                    else if(xFinal >= 11 && yFinal <= 10) // point d'arrivée dans le marécage 
                    {
                        int xFIntermediaire = 11;
                        int yFIntermediaire = 0;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaire, yFIntermediaire) 
                            + 3*Euclidienne(xFIntermediaire, yFIntermediaire,xFinal, yFinal);
                    }
                    else if(xFinal >= 11 && yFinal >= 10) //point d'arrivée en bas à droite 
                    {
                        int xFIntermediaire = 11;
                        int yFIntermediaire = 0;

                        int xFSortieMarecage = 11;
                        int yFSortieMarecage = 10 ;

                        distanceEucli = 
                            Euclidienne(x,y,xFIntermediaire, yFIntermediaire)  
                            + 3*Euclidienne(xFIntermediaire, yFIntermediaire,xFSortieMarecage, yFSortieMarecage) 
                            + Euclidienne(xFSortieMarecage, yFSortieMarecage,xFinal, yFinal);
                    }
                }
                else if (x > 10) // à droite - 10 exclu (Partie droite)
                {
                    if (y <= 9) // Zone 4 : Zone marécageuse
                    {
                        if ((xFinal > 10 && yFinal <= 9)) // deux points dans la zone 4 
                        {
                            distanceEucli = 3*Euclidienne(x,y,xFinal, yFinal);
                        }
                        else if (xFinal > 2 && xFinal <= 9 && yFinal >= 4 && yFinal <= 9) // point d'arrivée dans le cercle
                        {

                            int xFIntermediaireGoulot = 10;
                            int yFIntermediaireGoulot = 0;

                            int xFIntermediaireZone3 = 1;
                            int yFIntermediaireZone3 = 4;

                            int xFIntermediaireSortieCercle = 2;
                            int yFIntermediaireSortieCercle = 6;

  

                            distanceEucli = 
                                3* Euclidienne(x,y,xFIntermediaireGoulot, yFIntermediaireGoulot) 
                                + Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot,xFIntermediaireZone3, yFIntermediaireZone3)
                                + Euclidienne(xFIntermediaireZone3, yFIntermediaireZone3, xFIntermediaireSortieCercle, yFIntermediaireSortieCercle)
                                + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle,xFinal, yFinal);
                        }
                        else if (xFinal >= 0 && xFinal <= 9 && yFinal >= 11) // point d'arrivée en bas à gauche 
                        {
                            int xFIntermediaireGoulot = 10;
                            int yFIntermediaireGoulot = 0;

                            int xFIntermediaireZone3 = 1;
                            int yFIntermediaireZone3 = 4;

                            int xFIntermediaire2 = 1;
                            int yFIntermediaire2 = 9;

                            distanceEucli = 
                                3*Euclidienne(x,y,xFIntermediaireGoulot, yFIntermediaireGoulot) 
                                + Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot,xFIntermediaireZone3, yFIntermediaireZone3) 
                                + Euclidienne(xFIntermediaireZone3, yFIntermediaireZone3,xFIntermediaire2, yFIntermediaire2)  
                                + Euclidienne(xFIntermediaire2, yFIntermediaire2,xFinal, yFinal);
                        }
                        else if (xFinal >= 0 && xFinal <=9 && yFinal >= 0 && yFinal <= 2) // point d'arrivée en haut à gauche
                        {
                            int xFIntermediaireGoulot = 10;
                            int yFIntermediaireGoulot = 0;

                            distanceEucli = 
                                3*Euclidienne(y,y,xFIntermediaireGoulot, yFIntermediaireGoulot) 
                                + Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot,xFinal, yFinal);
                        }
                        else if (xFinal >= 11 && yFinal >= 10) //point d'arrivée en bas à droite 
                        {
                            int xFSortieMarecage = x;
                            int yFSortieMarecage = 10;

                            distanceEucli =
                                3*Euclidienne(x,y,xFSortieMarecage, yFSortieMarecage) 
                                + Euclidienne(xFSortieMarecage, yFSortieMarecage,xFinal, yFinal);
                        }
                    }

                    else //Zone 5 : En dessous de la zone marécage
                    {
                        if ((xFinal > 10 && yFinal >= 10)) // deux points dans la zone 5 
                        {
                            distanceEucli = Euclidienne(x,y,xFinal, yFinal);
                        }
                        else if (xFinal > 2 && xFinal <= 9 && yFinal >= 4 && yFinal <= 9) // point d'arrivée dans le cercle
                        {
                            int xFSortieMarecage = 11;
                            int yFSortieMarecage = 9;

                            int xFIntermediaireGoulot = 10;
                            int yFIntermediaireGoulot = 0;

                            int xFIntermediaireZone3 = 1;
                            int yFIntermediaireZone3 = 4;

                            int xFIntermediaireSortieCercle = 2;
                            int yFIntermediaireSortieCercle = 6;

                            distanceEucli = 
                                Euclidienne(x,y,xFSortieMarecage, yFSortieMarecage) 
                                + 3*Euclidienne(xFSortieMarecage, yFSortieMarecage,xFIntermediaireGoulot, yFIntermediaireGoulot) 
                                + Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot,xFIntermediaireZone3, yFIntermediaireZone3)
                                + Euclidienne(xFIntermediaireZone3, yFIntermediaireZone3, xFIntermediaireSortieCercle, yFIntermediaireSortieCercle)
                                + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle,xFinal, yFinal);
                        }
                        else if (xFinal >= 0 && xFinal <= 10 && yFinal >= 11) // point d'arrivée en bas à gauche 
                        {
                            int xFSortieMarecage = 11;
                            int yFSortieMarecage = 9;
                            
                            int xFIntermediaireGoulot = 10;
                            int yFIntermediaireGoulot = 0;

                            int xFIntermediaireZone3 = 1;
                            int yFIntermediaireZone3 = 4;

                            int xFIntermediaire2 = 1;
                            int yFIntermediaire2 = 9;

                            distanceEucli = 
                                Euclidienne(x,y,xFSortieMarecage, yFSortieMarecage) 
                                + 3*Euclidienne(xFSortieMarecage, yFSortieMarecage,xFIntermediaireGoulot, yFIntermediaireGoulot)
                                + Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot,xFIntermediaireZone3, yFIntermediaireZone3)
                                + Euclidienne(xFIntermediaireZone3, yFIntermediaireZone3,xFIntermediaire2, yFIntermediaire2)
                                + Euclidienne(xFIntermediaire2, yFIntermediaire2,xFinal, yFinal);
                        }
                        else if (xFinal >= 0 && xFinal <=10 && yFinal >= 0 && yFinal <= 2) // point d'arrivée en haut à gauche
                        {
                            int xFSortieMarecage = 11;
                            int yFSortieMarecage = 9;

                            int xFIntermediaireGoulot = 10;
                            int yFIntermediaireGoulot = 0;

                            distanceEucli = 
                                Euclidienne(x,y,xFSortieMarecage, yFSortieMarecage) 
                                + 3*Euclidienne(xFSortieMarecage, yFSortieMarecage,xFIntermediaireGoulot, yFIntermediaireGoulot) 
                                + Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot,xFinal, yFinal);
                        }
                        else if (xFinal >= 11 && yFinal <= 9) //point d'arrivée au marécage
                        {
                            int xFSortieMarecage = xFinal;
                            int yFSortieMarecage = 9;

                            distanceEucli =
                                Euclidienne(x,y,xFSortieMarecage, yFSortieMarecage) 
                                + 3*Euclidienne(xFSortieMarecage, yFSortieMarecage,xFinal, yFinal);
                        }                    
                    }
                }
                else if (x >=0 && x<= 3 && y>=3 && y <= 10) //Zone 6 : partie voisine de la HG et BG
                {
                    if ((xFinal >=0 && x<= 3 && yFinal>=3 && yFinal <= 10)) // point arrivée dans le connector
                    {
                        distanceEucli = Euclidienne(x,y,xFinal, yFinal);
                    }
                    else if (xFinal > 2 && xFinal <= 9 && yFinal >= 4 && yFinal <= 9) // point d'arrivée dans le cercle
                    {
                        int xFIntermediaireSortieCercle = 2;
                        int yFIntermediaireSortieCercle = 6;

                        distanceEucli =
                            Euclidienne(x,y,xFIntermediaireSortieCercle, yFIntermediaireSortieCercle)
                            + Euclidienne(xFIntermediaireSortieCercle, yFIntermediaireSortieCercle,xFinal, yFinal);
                    }
                    else if (xFinal >= 0 && xFinal <= 9 && yFinal >= 11) // point d'arrivée en bas à gauche 
                    { 

                        int xFIntermediaire2 = 1;
                        int yFIntermediaire2 = 9;

                        distanceEucli =
                            Euclidienne(x,y,xFIntermediaire2, yFIntermediaire2)
                            + Euclidienne(xFIntermediaire2, yFIntermediaire2,xFinal, yFinal);
                    }
                    else if (xFinal >= 0 && xFinal <= 9 && yFinal >= 0 && yFinal <= 2) // point d'arrivée en haut à gauche
                    {
                        int xFIntermediaireZone3 = 1;
                        int yFIntermediaireZone3 = 4;

                        distanceEucli =
                            Euclidienne(x,y,xFIntermediaireZone3, yFIntermediaireZone3)
                            + Euclidienne(xFIntermediaireZone3, yFIntermediaireZone3,xFinal, yFinal);
                    }
                    else if (xFinal >= 11 && yFinal <= 9) //point d'arrivée au marécage
                    {
                        int xFIntermediaireZone3 = 1;
                        int yFIntermediaireZone3 = 4;

                        int xFIntermediaireGoulot = 11;
                        int yFIntermediaireGoulot = 0;

                        distanceEucli =
                            Euclidienne(x, y, xFIntermediaireZone3, yFIntermediaireZone3)
                            + Euclidienne(xFIntermediaireZone3, yFIntermediaireZone3, xFIntermediaireGoulot, yFIntermediaireGoulot)
                            +3* Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot, xFinal, yFinal);
                    }
                    else if (xFinal >= 11 && yFinal >= 10) //point d'arrivée en bas à droite 
                    {
                        int xFIntermediaireZone3 = 1;
                        int yFIntermediaireZone3 = 4;

                        int xFIntermediaireGoulot = 11;
                        int yFIntermediaireGoulot = 0;

                        int xFSortieMarecage = 11;
                        int yFSortieMarecage = 10;

                        distanceEucli =
                            Euclidienne(x,y,xFIntermediaireZone3, yFIntermediaireZone3)
                            + Euclidienne(xFIntermediaireZone3, yFIntermediaireZone3,xFIntermediaireGoulot, yFIntermediaireGoulot)
                            + 3*Euclidienne(xFIntermediaireGoulot, yFIntermediaireGoulot,xFSortieMarecage, yFSortieMarecage)
                            + Euclidienne(xFSortieMarecage, yFSortieMarecage,xFinal, yFinal);
                    }
                }

            }
            // Heuristique de l'environnement 1
            // --------------------------------------
            else // ENV 1
            // --------------------------------------
            {
                distanceEucli = Euclidienne(x,y,xFinal, yFinal);
                
            }
            return ( distanceEucli );           
        }

        public override string ToString()
        {
            return Convert.ToString(x)+","+ Convert.ToString(y);
        }
    }
}