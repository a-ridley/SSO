<template>
  <v-layout id="login" xs>
    <div id="login">
      <h1 class="display-1">Login</h1>
      <v-divider class="my-3"/>
      <v-flex>
        <v-form>
        <v-text-field
          name="email"
          id="email"
          v-model="email"
          type="email"
          label="Email" 
          /><br />
        <v-text-field
          name="password"
          id="password"
          type="password"
          v-model="password"
          label="Password" 
        />
        <br/>
        <v-alert
          :value="error"
          type="error"
          transition="scale-transition"
        >
          {{error}}
        </v-alert>
        </v-form>
      </v-flex>
      <v-flex>
        <v-btn id="resetButton" color="success" flat small v-on:click="goToResetPassword">Reset Password</v-btn>
      </v-flex>
      <v-flex>
        <v-btn id="newuserButton" color="primary" flat small v-on:click="goToRegisterPage">New User? Register Here</v-btn>
      </v-flex>
      <v-flex>
        <v-btn id="loginButton" color="success" v-on:click="login">Login</v-btn>
      </v-flex>
      <v-dialog
        v-model="loading"
        hide-overlay
        persistent
        width="300"
      >
      <v-card
        color="primary"
        dark
      >
        <v-card-text>
          Signing in...
          <v-progress-linear
            indeterminate
            color="white"
            class="mb-0"
          ></v-progress-linear>
        </v-card-text>
      </v-card>
    </v-dialog>
    </div>
  </v-layout>
</template>

<script>
    import axios from "axios"
    import { apiURL } from '@/const.js'
    import { store } from '@/services/request'
    
    export default {
        name: 'login',
        data() {
            return {
                email: "",
                password: "",
                error: "",
                loading: false
            }
        },
        methods: {
            login() {
               const url = `${apiURL}/users/login`
              this.loading = true;
               axios.post(url,
               {
                    email: this.$data.email,
                    password: this.$data.password
               })
               .then(resp => {
                   let respData = resp.data
                   localStorage.setItem('token', respData)
                   store.state.isLogin = true
                   store.getEmail()
                   this.$router.push('/dashboard')
                })
               .catch(e => {
                    if(e.response.status === 400){
                        this.error = "Invalid Username/Password"
                    }
                    else if(e.response.status === 401){
                        this.error = "User is Disabled"
                    }
                    else{
                        this.error = e.response.data
                    }
            })
            .finally(() => {
                this.loading = false;
            })
        },
        goToResetPassword(){
            this.$router.push('/sendresetlink');
        },
        goToRegisterPage(){
          this.$router.push('/register');
        }
    }
}
</script>

<style>
#login{
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}

#loginButton {
  margin: 0px
}

#resetButton {
  margin: 0px;
  margin-bottom: 15px;
  padding: 0px;
}

#newuserButton {
  margin: 0px;
  margin-bottom: 15px;
  padding: 0px;
}
</style>
