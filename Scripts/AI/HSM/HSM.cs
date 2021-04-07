using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class HSM : MonoBehaviour
{
    private Dictionary<Type, HSMBaseState> states;
    public HSMBaseState currentState;

    public HSMBaseState CurrentState
    {
        //read what the current state is
        get
        {
            return currentState;
        }
        //write the value to the current state
        private set
        {
            currentState = value;
        }
    }

    //Set the state from the Dictionary states. 
    public void SetStates(Dictionary<Type, HSMBaseState> states)
    {
        //Set the state of this object to current value of Dictionary states.
        this.states = states;
    }


    // Update is called once per frame
    void Update()
    {
        //call CurrentState function to read what the current state is
        //if it is not set to anything, then set to the first value of the states Dictionary and have it enter that state.
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
            CurrentState.EnterState();
        }
        else
        {
            //check what's happening in the UpdateState and set its value to a new var nextState
            var nextState = CurrentState.UpdateState();

            //if nextState has a value(or state) and that value isn't the same as value as the current state.
            if (nextState != null && nextState != CurrentState.GetType())
            {
                //then switch to this new nextState.
                SwitchToState(nextState);
            }
        }
    }

    //Function to switch to the next state, with parameter holding value of that next state.
    //tells current state to exit, then updates current state to the nextState in the states list
    //and finally then enters this newly updated current state.
    void SwitchToState(Type nextState)
    {
        CurrentState.ExitState();
        CurrentState = states[nextState];
        CurrentState.EnterState();
    }
}
