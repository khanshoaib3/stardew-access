namespace stardew_access.Features
{
    public class Warnings
    {
        public void update()
        {
            this.checkForHealth();
            this.checkForStamina();
        }

        public void checkForStamina()
        {
            int stamina = CurrentPlayer.Stamina;

            if (stamina <= 50)
            {
                // 50% stamina warning
            }
            else if (stamina <= 25)
            {
                // 25% stamina warning
            }
            else if (stamina <= 10)
            {
                // 10% stamina warning
            }
        }

        public void checkForHealth()
        {
            int health = CurrentPlayer.Health;

            if (health <= 50)
            {
                // 50% health warning
            }
            else if (health <= 25)
            {
                // 25% health warning
            }
            else if (health <= 10)
            {
                // 10% health warning
            }
        }
    }
}