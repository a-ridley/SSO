<template>
  <v-dialog v-model="thisDialog" max-width="500px">
    <v-card>
      <v-card-title>
        <span>{{ text }}</span>
        <v-spacer></v-spacer>
      </v-card-title>
      <v-card-actions>
        <v-btn color="primary" flat @click="HandleDialog()">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  name: "PopupDialog",
  data: () => ({
    thisDialog: true,
    redirectOnClick: false,
    routeOnClick: false,
  }),
  props: {
    dialog: Boolean,
    text: String,
    redirect: Boolean,
    redirectUrl: {
      type: String,
      required: false,
      default: ''
    },
    route: Boolean,
    routeTo: {
      type: String,
      required: false,
      default: '/'
    }
  },
  created() {
    this.thisDialog = this.$props.dialog;
    this.redirectOnClick = this.$props.redirect;
    this.routeOnClick = this.$props.route;
  },
  methods: {
    HandleDialog(){
      this.thisDialog = false;
      if (this.redirectOnClick){
        window.location.href = decodeURIComponent(this.$props.redirectUrl);
      }
      if (this.routeOnClick){
        this.$router.push(this.$props.routeTo);
      }
    }
  },
}
</script>