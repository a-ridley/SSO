<template>
  <v-layout id="appDelete">
    <div id="appDelete">
      <h1 class="display-1">Delete Application</h1>
      <v-divider class="my-3"></v-divider>
      <br />
      <v-form>
      <v-text-field
          name="title"
          id="title"
          v-model="title"
          type="title"
          label="Application Title" 
          v-if="!validation"
          /><br />
      <v-text-field
          name="email"
          id="email"
          type="email"
          v-model="email"
          label="Email" 
          v-if="!validation"
          /><br />

      
      <v-alert
          :value="error"
          id="error"
          type="error"
          transition="scale-transition"
      >
          {{error}}
      </v-alert>

      <div v-if="validation" id="deleteMessage">
          <h3>{{ validation }}</h3>
      </div>

      <br />

      <v-btn id="btnDelete" color="success" v-if="!validation" v-on:click="deleteApp">Delete</v-btn>

      </v-form>

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
            Loading
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
import axios from 'axios'
import { apiURL } from '@/const.js'

export default {
  data () {
    return {
      validation: null,
      title: '',
      email: '',
      error: '',
      loading: false
    }
  },
  methods: {
    deleteApp: function () {
      
      this.error = "";
      if (this.title.length == 0 || this.email.length == 0) {
        this.error = "Fields Cannot Be Left Blank.";
      }

      if (this.error) return;

      const url = `${apiURL}/applications/delete`
      this.loading = true;
      axios.post(url, {
        title: document.getElementById('title').value,
        email: document.getElementById('email').value,
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      })
        .then(response => {
            this.validation = response.data.Message; // Retrieve deletion validation
        })
        .catch(err => {
            this.error = err.response.data.Message
        })
        .finally(() => {
          this.loading = false;
        })
    }
  }
}

</script>

<style lang="css">
#appDelete {
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}

</style>
