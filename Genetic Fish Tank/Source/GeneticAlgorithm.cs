namespace Genetic_Fish_Tank.Source
{
    class GeneticAlgorithm
    {
        public int generation = 0;
        public double generationTimer = 0;

        public Fish[] RankFittest(Fish[] fishList)
        {
            Fish[] newFishList = fishList;

            Fish temp;

            for (int x = 0; x < fishList.Length; x++)
            {
                for (int y = 1; y < fishList.Length - x; y++)
                {
                    if (newFishList[y - 1].collisionCircle.Score < newFishList[y].collisionCircle.Score)
                    {
                        temp = newFishList[y - 1];
                        newFishList[y - 1] = newFishList[y];
                        newFishList[y] = temp;
                    }
                }
            }

            return newFishList;
        }
    }
}
