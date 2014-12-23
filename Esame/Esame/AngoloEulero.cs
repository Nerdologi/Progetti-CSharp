using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esame
{
    class AngoloEulero
    {
        float yaw;
        float pitch;
        float roll;

        public AngoloEulero(float yaw, float pitch, float roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }

        public void setYaw(float value)
        {
            this.yaw = value;
        }

        public void setPitch(float value)
        {
            this.pitch = value;
        }

        public void setRoll(float value)
        {
            this.roll = value;
        }

        public float getYaw()
        {
            return this.yaw;
        }

        public float getPitch()
        {
            return this.pitch;
        }

        public float getRoll()
        {
            return this.roll;
        }
    }
}
