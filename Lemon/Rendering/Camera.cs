﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lemon.Rendering;
internal sealed class Camera {
    public Camera(float vFov, float nearClip, float farClip) {
        Fov = vFov;
        NearClip = nearClip;
        FarClip = farClip;

        ForwardDirection = new Vector3(0f, 0f, -1f);
        Position = new Vector3(0f, 0f, 3f);
    }

    public float NearClip { get; init; }
    public float FarClip { get; init; }
    public float Fov { get; private set; }
    public Vector3 ForwardDirection { get; private set; }
    public Vector3 Position { get; private set; }
    public float RotationSpeed { get; set; } = 0.1f;

    public bool OnUpdate(float deltaTime, MouseState mouseState, KeyboardState keyboardState, out CursorState cursorState) {
        if (!mouseState.IsButtonDown(MouseButton.Right)) {
            cursorState = CursorState.Normal;
            return false;
        }
        
        Vector3 upDir = new Vector3(0f, 1f, 0f);
        Vector3 rightDir = Vector3.Cross(ForwardDirection, upDir);

        float speed = 5f;
        if (keyboardState.IsKeyDown(Keys.W))
            Position += ForwardDirection * speed * deltaTime;
        else if (keyboardState.IsKeyDown(Keys.S))
            Position -= ForwardDirection * speed * deltaTime;
        if (keyboardState.IsKeyDown(Keys.A))
            Position -= rightDir * speed * deltaTime;
        else if (keyboardState.IsKeyDown(Keys.D))
            Position += rightDir * speed * deltaTime;
        if (keyboardState.IsKeyDown(Keys.Q))
            Position -= upDir * speed * deltaTime;
        else if (keyboardState.IsKeyDown(Keys.E))
            Position += upDir * speed * deltaTime;

        if (keyboardState.IsKeyReleased(Keys.Z))
            Fov -= 3;
        else if (keyboardState.IsKeyReleased(Keys.X))
            Fov += 3;

        Vector2 delta = mouseState.Delta;
        if (delta.X != 0f || delta.Y != 0f) {
            float pitchDelta = -delta.Y * RotationSpeed;
            float yawDelta = -delta.X * RotationSpeed;

            ForwardDirection = Rotate(ForwardDirection, pitchDelta, yawDelta);
        }

        cursorState = CursorState.Grabbed;
        return true;
    }

    public float DegToRad(float deg) {
        return deg * MathF.PI / 180;
    }

    public Vector3 Rotate(Vector3 vector, float pitchDelta, float yawDelta) {
        float pitch = DegToRad(pitchDelta);
        float yaw = DegToRad(yawDelta);

        Quaternion pitchRotation = Quaternion.FromAxisAngle(Vector3.UnitX, pitch);
        Quaternion yawRotation = Quaternion.FromAxisAngle(Vector3.UnitY, yaw);

        Quaternion rotation = pitchRotation * yawRotation;

        Vector3 transformed = Vector3.Transform(vector, rotation);
        float y = vector.Y + pitchDelta * RotationSpeed / 10;
        y = MathF.Min(0.4f, MathF.Max(y, -0.4f));
        return new Vector3(transformed.X, y, transformed.Z);
    }
}
