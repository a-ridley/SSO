<template>
  <v-layout id="reset" xs>
    <div id="reset">
      <h1 class="display-1">Password Reset</h1>
      <v-divider class="my-3"/>
      <v-alert
      :value="message"
      dismissible
      type="success"
      transition="scale-transition"
    >
    {{message}}
    </v-alert>

    <v-alert
      :value="errorMessage"
      dismissible
      type="error"
      transition="scale-transition"
    >
    {{errorMessage}}
    </v-alert>

    <v-alert
      v-model="wrongAnswerAlert"
      dismissable
      type="error"
       transition="scale-transition"
    >
    One or more of the answers are incorrect
    </v-alert>

    <br />
    <div class="SecurityQuestions" v-if="securityQuestions.length">
      <br/>
      <div v-for="(securityQuestion, index) in securityQuestions" :key="index">
        {{securityQuestion}}
      </div>
      <br />
      <v-text-field
            name="SecurityAnswer1"
            id="SecurityAnswer1"
            v-model="securityAnswer1"
            type="text"
            label="Answer for Question 1"/>
      <br />

      <v-text-field
            name="SecurityAnswer2"
            id="SecurityAnswer2"
            v-model="securityAnswer2"
            type="text"
            label="Answer for Question 2"/>
      <br />

      <v-text-field
            name="SecurityAnswer3"
            id="SecurityAnswer3"
            v-model="securityAnswer3"
            type="text"
            label="Answer for Question 3"/>
      <br />
      <v-btn id="submitAnswers" color="success" v-on:click="submitAnswers">Submit Answers</v-btn>
    </div>

    <br/>

    <div id="NewPassword" v-if="showPasswordResetField">
      Enter a new password into the field
      <br/>
      <v-text-field
            name="Password"
            id="Password"
            v-model="newPassword"
            type="text"
            label="New Password"/>
      <br />
      <v-text-field
            name="ConfirmPassword"
            id="ConfirmPassword"
            v-model="confirmNewPassword"
            type="text"
            label="Cofirm New Password"/>
      <br />
      <v-btn id="submitPassword" color="success" v-on:click="submitNewPassword">Submit New Password</v-btn>
    </div>
    <Loading :dialog="loading" :text="loadingText" />
    </div>
  </v-layout>
</template>

<script>
import axios from 'axios'
import { apiURL } from '@/const.js';
import Loading from '@/components/Dialogs/Loading'

export default {
  name: 'ResetPassword',
  components:{
    Loading,
  },
  data () {
    return {
      resetToken: this.$route.params.id,
      message: null,
      errorMessage: null,
      securityQuestions: {
        securityQuestion1: null,
        securityQuestion2: null,
        securityQuestion3: null
      },
      securityAnswer1: null,
      securityAnswer2: null,
      securityAnswer3: null,
      showPasswordResetField: null,
      confirmNewPassword: null,
      newPassword: null,
      networkErrorMessage: null,
      haveNetworkError: false,
      wrongAnswerCounter : 0,
      wrongAnswerAlert: null,
      loading: false,
      loadingText: "",
    }
  },
  created () {
    this.loading = true;
    this.loadingText = "Loading...";
    axios({
      method: 'GET',
      url: `${apiURL}/reset/` + this.resetToken,
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Credentials': true
      }
    })
      .then(response => (this.securityQuestions = response.data))
      .catch(e => { this.errorMessage = e.response.data })
      .finally(() => {
        this.loading = false;
      })
  },
  methods: {
    redirectToReset: function () {
      this.$router.push( "/sendresetlink" )
    },
    redirectToLogin: function () {
      this.$router.push( "/login" )
    }, 
    submitAnswers: function () {
      if(this.wrongAnswerCounter === 3){
        this.errorMessage = "3 attempts have been made, reset link is no longer valid"
        this.$router.push("/SendResetLink")
      }
      if (!this.securityAnswer1 || !this.securityAnswer2 || !this.securityAnswer3){
        this.errorMessage = "Security answers cannot be empty"
      } else {
        this.loading = true;
        this.loadingText = "Checking Answers...";
        axios({
        method: 'POST',
        url: `${apiURL}/reset/` + this.resetToken + '/checkanswers',
        data: { 
          securityA1: this.$data.securityAnswer1,
          securityA2: this.$data.securityAnswer2,
          securityA3: this.$data.securityAnswer3},
        headers: {
          'Access-Control-Allow-Origin': '*',
          'Access-Control-Allow-Credentials': true
        }
      })
        .then(response => (
          this.showPasswordResetField = response.data))
        .catch(e => { this.errorMessage = e.response.data }, this.wrongAnswerCounter = this.wrongAnswerCounter + 1)
        .finally(() => {
          this.loading = false;
        })
      }
    },
    submitNewPassword: function () {
      if(this.newPassword === null){
        this.errorMessage = "Password cannot be empty"
      } else if (this.newPassword.length < 12){
        this.errorMessage = "Password must be at least 12 characters"
      } else if (this.newPassword.length > 2000) {
        this.errorMessage = "Password must be less than 2000 characters"
      } else if(this.newPassword != this.confirmNewPassword){
        this.errorMessage = "Passwords do not match"
      } else {
        this.errormessage = null
        this.loading = true;
        this.loadingText = "Resetting Password...";
        axios({
        method: 'POST',
        url: `${apiURL}/reset/` + this.resetToken + '/resetpassword',
        data: {newPassword: this.$data.newPassword},
        headers: {
          'Access-Control-Allow-Origin': '*',
          'Access-Control-Allow-Credentials': true
        }
      })
        .then(response => (this.message = response.data))
        .catch(e => { this.errorMessage = e.response.data })
        .finally(() => {
          this.loading = false;
        })
      }
    }
  }
}
</script>

<style>
#reset{
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}
</style>
