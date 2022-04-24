using System.Collections.Generic;

namespace Inversion
{
    public class PowerSourceHandler : ReactionHandler
    {
        private const int MaxRecursionDepth = 5;

        public bool IsPowered { get; set; } = false;

        private List<IMetallic> lastNetwork = new List<IMetallic>();
        private List<IMetallic> newNetwork = new List<IMetallic>();

        private BaseLevel level;

        public override void _Ready()
        {
            trackMetallics = true;
            level = (BaseLevel)GetTree().CurrentScene;
            base._Ready();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (IsPowered)
            {
                newNetwork = new List<IMetallic>();

                GetConnectionsRecursive(NearbyMetallics, 0);

                foreach (var connection in lastNetwork)
                {
                    if (!newNetwork.Contains(connection))
                        connection.UnPower();
                }

                foreach (var connection in newNetwork)
                {
                    connection.Power();
                }

                lastNetwork = newNetwork;
            }
            else if (lastNetwork.Count > 0)
            {
                foreach (var connection in lastNetwork)
                {
                    connection.UnPower();
                }

                lastNetwork.Clear();
            }
        }

        private void GetConnectionsRecursive(List<IMetallic> list, int currentDepth)
        {
            if (currentDepth >= MaxRecursionDepth)
                return;

            currentDepth++;
            foreach (var connection in list)
            {
                if (!newNetwork.Contains(connection))
                {
                    newNetwork.Add(connection);
                    GetConnectionsRecursive(connection.GetNearbyMetallics(), currentDepth);
                }
            }
        }
    }
}