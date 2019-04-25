<template>
  <div>
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
          Logging out...
          <v-progress-linear
            indeterminate
            color="white"
            class="mb-0"
          ></v-progress-linear>
        </v-card-text>
      </v-card>
    </v-dialog>
    <div v-if="true">
      <PopupDialog :dialog="true" :text="popupMessage" :redirect="false" :route="true" :routeTo="routeTo" />
    </div>
  </div>  
</template>

<script>
import axios from "axios"
import { apiURL } from "@/const.js"
import { store } from '@/services/request'
import PopupDialog from '@/components/Dialogs/PopupDialog.vue'

export default {
  name: "Logout",
  components: {
    PopupDialog
  },
  data() {
    return {
      loading: false,
      logoutSuccess: false,
      popupMessage: '',
      token: "",
      routeTo: '/home'
    }
  },
  created() {
    this.loading = true;
      const url = `${apiURL}/Logout`
      axios.post(url, 
        {
          token: localStorage.getItem('token')
        })
        .then(response => {
          this.routeTo = '/login';
          this.logoutSuccess = true;
          this.popupMessage = response.data;
          localStorage.removeItem('token');
          store.state.isLogin = false;
        })
        .catch(e => {
          if (e.response.status === 417) {
            this.routeTo = '/dashboard';
            this.logoutSuccess = true;
            this.popupMessage = "Logout has encountered an error."
          }
          else {
            this.routeTo = '/dashboard';
            this.logoutSuccess = true;
            this.popupMessage = e.response.data
          }
        })
        .finally(() => {
          this.loading = false;
        })
    
  }
};
</script>
