﻿using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Net;
using System.Net.Http.Headers;
using System.ComponentModel;

namespace AAAGR_io
{
    public class Eater : GameObject
    {
        public bool IsAI { get; private set; } = false;

        private CircleShape body;

        public Eater(bool isAi, float mass, string name, Color color) 
        {
            IsAI = isAi;

            body = new CircleShape(25f);

            body.Origin = new Vector2f(body.Radius, body.Radius);

            UniversalShape = body;

            body.FillColor = color;
            body.OutlineColor = Color.Black;
            body.OutlineThickness = 6;

            this.mass = mass;

            tag = "Eater";
            this.name = name;
        }
        public override CircleShape ActualPlayerShape()
            => body;
        public override void OnSoulChange()
        {
            if(IsAI)
                body.FillColor = Color.Magenta;
            else
                body.FillColor = Color.Black;

            IsAI = !IsAI;
        }
        public override ListedGameObject ChangeSoul()
        {
            List<ListedGameObject> players = Game.Instance.GameObjectsList.GetPlayerList();

            Random rand = new Random();

            ListedGameObject chosenOne;

            do
            {
                int index = rand.Next(0, players.Count);

                chosenOne = players[index];
            }
            while (chosenOne.GameObjectPair.Item1 == this.name || !chosenOne.GameObjectPair.Item2.isAlive);

            OnSoulChange();

            chosenOne.GameObjectPair.Item2.OnSoulChange();

            return chosenOne;
        }

        #region Overrides
        public override void Awake()
        {
            base.Awake();

            if (awakened)
                return;

            awakened = true;

            Random rand = new Random();

            int playerX = rand.Next(100, (int)Render.width - 100);
            int playerY = rand.Next(100, (int)Render.height - 100);

            body.Position = new Vector2f(playerX, playerY);

        }
        public override void Update()
        {
            base.Update();

            ControlMass();
        }
        public override void Destroy()
        {
            if (!IsAI)
            {
                Game.Instance.GameObjectsList.OnMainPlayerLoss();
                Game.Instance.AddScore(-1);
            }

            base.Destroy();
        }
        public override void Eat(GameObject food)
        {
            base.Eat(food);

            if (food.tag is "food")
                mass += (food.mass * 0.2f) / mass;
            else if (food.tag is "Eater")
                mass += food.mass * 0.025f;

            food.isAlive = false;

            Game.Instance.OnObjectDeath(food.name);
        }
        protected override void OnMassChanged()
        {
            UniversalShape.Scale = new Vector2f(mass, mass);

            if (!IsAI)
                Render.UpdateMassText(mass);
        }
        #endregion

        #region MassControl
        private void ControlMass()
        {
            if(mass >= 16.5f)
            {
                Game.Instance.OnObjectDeath(name);
                Game.Instance.AddScore(3);
            }
        }
        #endregion

        public override void Move(Vector2f newPosition)
        {
            base.Move(newPosition);

            body.Position = newPosition;
        }
    }
}
