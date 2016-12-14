using System;
using System.Drawing;

namespace TechEd.Demo.SolidPrinciples.Lsp.Principles.Entities
{
    public class Car
    {
        private bool _hasFuel = true;

        public Car(Color color)
        {
            Color = color;
        }

        public virtual void StartEngine()
        {
            if (!_hasFuel)
                throw new OutOfFuelException("Can't start a car without gas in tank...");

            IsEngineRunning = true;
        }
        public virtual void StopEngine()
        {
            IsEngineRunning = false;
        }

        public bool IsEngineRunning { get; private set; }
        public Color Color { get; protected set; }
    }

    public class BrokenCar : Car
    {
        public BrokenCar(Color color) : base(color)
        {
        }

        public override void StartEngine()
        {
            throw new NotImplementedException();
        }
    }

    public class CrimeBossCar : Car
    {
        private readonly bool _boobyTrapped;

        public CrimeBossCar(Color color, bool boobyTrap)
            : base(color)
        {
            _boobyTrapped = boobyTrap;
        }

        public override void StartEngine()
        {
            if (_boobyTrapped)
                throw new MetYourMakerException("Boom! You are dead!");

            base.StartEngine();
        }
    }

    public class Prius : Car
    {
        public Prius(Color color) : base(color)
        {
        }

        public override void StartEngine()
        {
        }
        public override void StopEngine()
        {
        }
    }

    public class StolenCar : Car
    {
        private bool _ignitionWiresStripped;

        public StolenCar(Color color) : base(color)
        {
        }

        public void StripIgnitionWires()
        {
            _ignitionWiresStripped = true;
        }
        public override void StartEngine()
        {
            if (!_ignitionWiresStripped) return;

            base.StartEngine();
        }
    }

    // http://www.dailymail.co.uk/sciencetech/article-2451931/Car-colour-heat-sensitive-paint-changes-depending-weather.html
    public class PimpedCar : Car
    {
        private readonly Color _color;

        public PimpedCar(Color color) : base(color)
        {
            _color = color;
        }

        public void SetTemperature(int temp)
        {
            if (temp > 20)
                Color = _color;
            else
                Color = Color.Black;
        }
    }


    public class OutOfFuelException : Exception
    {
        public OutOfFuelException(string message) : base(message)
        {
            
        }
    }
    public class MetYourMakerException : Exception
    {
        public MetYourMakerException(string message) : base(message)
        {

        }
    }
}
