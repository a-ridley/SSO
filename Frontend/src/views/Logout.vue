<template>
  <div>
    <Loading :dialog="loading" :text="loadingText" />
    <div v-if="showPopup">
      <PopupDialog :dialog="showPopup" :text="popupMessage" :redirect="false" :route="true" :routeTo="routeTo" />
    </div>
  </div>  
</template>

<script>
import axios from "axios"
import { apiURL } from "@/const.js"
import { store } from '@/services/request'
import PopupDialog from '@/components/Dialogs/PopupDialog.vue'
import Loading from "@/components/Dialogs/Loading.vue";

export default {
  name: "Logout",
  components: {
    PopupDialog,
    Loading
  },
  data() {
    return {
      loading: false,
      loadingText: "",
      showPopup: false,
      popupMessage: '',
      token: "",
      routeTo: '/home'
    }
  },
  created() {
    this.loading = true;
    this.loadingText = "Logging out...";
    const token = localStorage.getItem("token");
    if(!token){
      this.routeTo = '/login';
      this.showPopup = true;
      this.popupMessage = "User has logged out.";   
    }
    const url = `${apiURL}/Logout`;
    axios.post(url, 
      {
        "token": token
      })
      .then(response => {
        this.routeTo = '/login';
        this.showPopup = true;
        this.popupMessage = response.data;
        localStorage.removeItem('token');
        store.state.isLogin = false;
      })
      .catch(e => {
        if (e.response.status === 417) {
          this.routeTo = '/login';
          this.showPopup = true;
          this.popupMessage = "An error has been encounted. User will be logged out.";
          localStorage.removeItem('token');
        }
        else {
          this.routeTo = '/login';
          this.showPopup = true;
          this.popupMessage = e.response.data;
          localStorage.removeItem('token');
        }
      })
      .finally(() => {
        this.loading = false;
        store.state.isLogin = false;
      })
  }
};
</script>
