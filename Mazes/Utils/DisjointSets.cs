using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mazes.Utils
{
   /*
    * The disjoint-set ADT is a collection of n elements partitioned in disjoint sets. Using 
    * two special heuristics (union by rank and path compression) and an array representation 
    * of disjoint sets, a sequence of m make_set, union, and find operations, n of which are 
    * make_set operations, takes O(m α(n)) time.
    */
    public class DisjointSets
    {
        public struct Set
        {
            public readonly int parent, rank;

            public Set(int parent, int rank)
            {
                this.parent = parent;
                this.rank = rank;
            }
        }

        private int size = 0;
        private List<Set> sets;

        public DisjointSets(int initialSize)
        {
            sets = new List<Set>(initialSize);
            for (int i = 0; i < initialSize; ++i)
            {
                MakeSet();
            }
        }

       /*
        * Returns the representative/root of the set containing x. 
        * Uses path compression heuristic to improve time complexity.
        */
        public int Find(int x)
        {
            // (Path Compression)
            // Find the root of x, and make the root the parent of x.
            if (sets[x].parent != x)
            {
                sets[x] = new Set(Find(sets[x].parent), sets[x].rank);
            }

            return sets[x].parent;
        }

       /*
        * Creates a new single element set represented as a root node, and returns its index.
        */
        public int MakeSet()
        {
            ++size;
            sets.Add(new Set(size - 1, 0));

            return size - 1;
        }

       /*
        * Merges sets that contain x and y, say Sx and Sy, forming a new set R = Sx ∪ Sy. 
        * Uses union by rank heuristic to improve time complexity.
        */ 
        public void Union(int x, int y)
        {
            int xRoot = Find(x);
            int yRoot = Find(y);

            // (Union by Rank)
            // Attach smaller rank tree under root of higher rank tree.
            // Otherwise if ranks are equal, then make one the root of the other, and 
            // increment the root's rank.
            if (sets[xRoot].rank < sets[yRoot].rank)
            {
                sets[xRoot] = new Set(yRoot, sets[xRoot].rank);
            }
            else if (sets[xRoot].rank > sets[yRoot].rank)
            {
                sets[yRoot] = new Set(xRoot, sets[yRoot].rank);
            }
            else
            {
                sets[yRoot] = new Set(xRoot, sets[yRoot].rank);
                sets[xRoot] = new Set(sets[xRoot].parent, sets[xRoot].rank + 1);
            }
        }
    }
}