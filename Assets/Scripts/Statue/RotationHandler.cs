namespace Statue
{
    public class RotationHandler
    {
        private float _rotationSpeed;

        // Constructor: This sets the speed when we create the handler
        public RotationHandler(float speed)
        {
            _rotationSpeed = speed;
        }

        // This is the "Pure" logic. 
        // It takes input and time, then returns how much to rotate.
        public float CalculateRotation(float input, float deltaTime)
        {
            return input * _rotationSpeed * deltaTime;
        }
    }
}