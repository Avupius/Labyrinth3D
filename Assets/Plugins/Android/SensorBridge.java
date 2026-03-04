package com.unity3d.player;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.util.Log;

public class SensorBridge implements SensorEventListener {
    private static float[] acceleration = {0, 0, 0};
    private SensorManager sensorManager;
    private static final String TAG = "UnityAccel";

    public SensorBridge(Context context) {
        sensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
        Sensor accel = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        if (accel != null) {
            sensorManager.registerListener(this, accel, SensorManager.SENSOR_DELAY_GAME);
            Log.d(TAG, "Accelerometer registered");
        }
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        acceleration[0] = event.values[0];
        acceleration[1] = event.values[1];
        acceleration[2] = event.values[2];
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {}

    public static float[] getAcceleration() {
        return acceleration;
    }
}