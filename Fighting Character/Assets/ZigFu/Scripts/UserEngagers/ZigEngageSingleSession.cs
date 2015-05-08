﻿using UnityEngine;
using System;
using System.Collections.Generic;

class ZigEngageSingleSession : MonoBehaviour {
    public GameObject EngagedUser;
    public List<GameObject> listeners = new List<GameObject>();
    Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();
   
    public bool StartOnSteady = false;
    public bool StartOnWave = true;
    public bool RotateToUser = true;


    //bounds in mm
    public Vector3 SessionBoundsOffset = new Vector3(0, 250, -300);
    public Vector3 SessionBounds = new Vector3(1500, 700, 1000);


    ZigTrackedUser engagedTrackedUser;

    void Start() {
        // make sure we get zig events
        ZigInput.Instance.AddListener(gameObject);
    }

    bool EngageUser(ZigTrackedUser user) {
        if (null == engagedTrackedUser) {
            engagedTrackedUser = user;
            if (null != EngagedUser) user.AddListener(EngagedUser);
            SendMessage("UserEngaged", this, SendMessageOptions.DontRequireReceiver);
            return true;
        }
        return false;
    }

    bool DisengageUser(ZigTrackedUser user) {
        if (user == engagedTrackedUser) {
            if (null != EngagedUser) user.RemoveListener(EngagedUser);
            engagedTrackedUser = null;
            SendMessage("UserDisengaged", this, SendMessageOptions.DontRequireReceiver);
            return true;
        }
        return false;
    }

    void Zig_UserFound(ZigTrackedUser user) {
        // create gameobject to listen for events for this user
        GameObject go = new GameObject("WaitForEngagement" + user.Id);
        go.transform.parent = transform;
        objects[user.Id] = go;

        user.AddListener(go);
    }

    void Zig_UserLost(ZigTrackedUser user) {
        DisengageUser(user);
        Destroy(objects[user.Id]);
        objects.Remove(user.Id);
    }

    public void Reset()
    {
        if (null != engagedTrackedUser) {
            DisengageUser(engagedTrackedUser);
        }
    }
}