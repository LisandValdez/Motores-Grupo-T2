using UnityEngine;

public class Estados : MonoBehaviour
{
    public abstract class State
    {
        protected GameObject enemy;

        public State(GameObject enemy)
        {
            this.enemy = enemy;
        }

        public abstract State RunCurrentState();
    }
}
