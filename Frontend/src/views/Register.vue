<template>
  <v-layout id="register">
    <div id="register">
      <h1 class="display-1">Register</h1>
      <v-divider class="my-3"/>
      <v-form>
        Credentials:
        <br />
        <v-text-field
          name="email"
          id="email"
          v-model="email"
          type="email"
          :rules="[rules.required, rules.emailRule]"
          label="Email"
           /><br />
        <v-text-field
          name="password"
          id="password"
          :type="show1 ? 'text' : 'password'"
          v-model="password"
          :append-icon="show1 ? 'visibility' : 'visibility_off'"
          :rules="[rules.required, rules.passwordRule]"
          label="Password"
          required
          @click:append="show1 = !show1" /><br />
        <v-text-field
          name="confirm"
          id="confirm"
          :type="show2 ? 'text' : 'password'"
          v-model="confirmPassword"
          :append-icon="show2 ? 'visibility' : 'visibility_off'"
          :rules="[rules.required, rules.PasswordRule, rules.confirmPasswordRule]"
          label="Confirm Password"
          required
          @click:append="show2 = !show2" /><br />

        <br /><br />
        Personal Details:<br />
        <v-menu
          ref="menu"
          v-model="menu"
          :close-on-content-click="false"
          :nudge-right="40"
          lazy
          transition="scale-transition"
          offset-y
          full-width
          min-width="290px"
        >
          <template v-slot:activator="{ on }">
            <v-text-field
              v-model="dob"
              label="Date of Birth"
              prepend-icon="event"
              :rules="[rules.required]"
              v-on="on"
              id="dob"
            ></v-text-field>
          </template>
          <v-date-picker
            ref="picker"
            v-model="dob"
            :max="new Date().toISOString().substr(0, 10)"
            min="1900-01-01"
            @change="updateDate"
          ></v-date-picker>
        </v-menu>

        <v-text-field
          name="city"
          id="city"
          v-model="city"
          label="City" /><br />
        <v-text-field
          name="state"
          id="state"
          v-model="state"
          label="State" /><br />
        <v-text-field
          name="country"
          id="country"
          v-model="country"
          label="Country" /><br />

        <br /><br />
        Security Questions:<br />
        <v-select
          :items="securityQuestions1"
          v-model="securityQ1"
          label="Security Question 1"
          id="securityq1"
          :rules="[rules.securityQuestionRule]"
        ></v-select>
        <br />
        <v-text-field
          name="securitya1"
          id="securitya1"
          v-model="securityQ1Answer"
          label="Security Answer 1"
          :rules="[rules.securityQuestionRule]" />
        <br />
        <br />
        <v-select
          :items="securityQuestions2"
          v-model="securityQ2"
          label="Security Question 2"
          id="securityq2"
          :rules="[rules.securityQuestionRule]"
        ></v-select><br />
        <v-text-field
          name="securitya2"
          id="securitya2"
          v-model="securityQ2Answer"
          label="Security Answer 2"
          :rules="[rules.securityQuestionRule]" /><br />

        <br />

        <v-select
          :items="securityQuestions3"
          v-model="securityQ3"
          label="Security Question 3"
          id="securityq3"
          :rules="[rules.securityQuestionRule]"
        ></v-select><br />
        <v-text-field
          name="securitya3"
          id="securitya3"
          v-model="securityQ3Answer"
          label="Security Answer 3"
          :rules="[rules.securityQuestionRule]" /><br />

        <v-flex>
          <v-btn id="legalButton" color="primary" flat small v-on:click="goToLegalPage">By registering, you're agreeing to our terms of service</v-btn>
        </v-flex>

        <v-alert
          :value="error"
          type="error"
          transition="scale-transition"
        >
          {{error}}
        </v-alert>
        <br />
        <v-btn id="registerButton" color="success" v-on:click="submit">Register</v-btn>

      </v-form>
      <Loading :dialog="loading" :text="loadingText" />
    </div>
  </v-layout>
</template>

<script>
import { register } from '@/services/request';
import { store } from '@/services/request';
import Loading from '@/components/Dialogs/Loading';

export default {
  name: 'Register',
  components:{
    Loading,
  },
  data() {
    return {
      show1: false,
      show2: false,
      show3: false,
      show4: false,

      rules: {
        required: v => !!v || 'This field is required.',
        emailRule: v => /\S+@.+\..+/.test(v) || 'E-mail must be valid.',
        passwordRule: v => (v && v.length >= 12) || 'Password must be at least 12 characters',
        confirmPasswordRule: v => (v && v === this.password) || "Passwords do not match.",
        securityQuestionRule:  v => !!v || "Security Question is required.",
      },

      menu: false,
      error: "",
      loading: false,
      loadingText: "",

      email: '',

      password: '',
      confirmPassword: '',

      dob: '',
      city: '',
      state: '',
      country: '',

      securityQ1: '',
      securityQ1Answer: '',
      securityQ2: '',
      securityQ2Answer: '',
      securityQ3: '',
      securityQ3Answer: '',

      securityQuestions1: ["What is your favorite pet's name?", "What is your mother's maiden name?", "What is your favorite superhero?"],
      securityQuestions2: ["What is your childhood best friend's name?", "In what city were you born?", "What is your favorite food?"],
      securityQuestions3: ["What is your favorite color?", "What is the make of your first car?", "In what city did you grow up?"],
    }
  },
  watch: {
    menu (val) {
      val && setTimeout(() => (this.$refs.picker.activePicker = 'YEAR'))
    }
  },
  methods: {
    updateDate(date) {
      this.$refs.menu.save(date);
    },
    submit: function() {
      this.error = "";
      if (this.password.length == 0) {
        this.error = "Password is required";
      } else if (this.password !== this.confirmPassword) {
        this.error = "Password entered does not match password confirmation";
      }

      if (this.error) return;

      this.loading = true;
      this.loadingText = "Registering...";
      register({
        email: this.email,
        password: this.password,
        confirmPassword: this.confirmPassword,

        dob: this.dob,
        city: this.city,
        state: this.state,
        country: this.country,

        securityQ1: this.securityQ1,
        securityQ1Answer: this.securityQ1Answer,
        securityQ2: this.securityQ2,
        securityQ2Answer: this.securityQ2Answer,
        securityQ3: this.securityQ3,
        securityQ3Answer: this.securityQ3Answer  
      }).then(() => {
        const params = new URLSearchParams(window.location.search)
        store.state.isLogin = true
        store.getEmail()
        if (params.has('redirect')) {
          window.location.href = decodeURIComponent(params.get('redirect'));
        } else {
          this.$router.push('dashboard');
        }
      }).catch(err => {
        switch((err.response || {}).status) {
          case 401:
            this.error = err.response.data;
            break;
          case 406:
            this.error = "Your email address does not follow a recognized format.";
            break;
          case 409:
            this.error = "This email address already belongs to an account.";
            break;
          case 412:
            this.error = "Please fill out all of the registration fields and try again.";
            break;
          default:
          case 500:
            this.error = "An unexpected server error occurred. Please try again momentarily.";
        }
      }).finally(() => {
        this.loading = false;
      })
    },
    goToLegalPage(){
      this.$router.push('/legal');
    },
  }
}
</script>

<style scoped>
#register {
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}

#registerButton {
  margin: 0px
}

#legalButton {
  margin: 0px;
  padding: 3px;
}
</style>
