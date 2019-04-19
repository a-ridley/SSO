<template>
  <v-dialog v-model="dialog" width="500">
    <template v-slot:activator="{ on }">
      <v-chip v-on="on" class="moreInfo" color="primary" text-color="white">More info</v-chip>
    </template>

    <v-card>
      <v-card-title class="headline grey lighten-2" primary-title>{{ title }}</v-card-title>

      <v-card-text>{{ description }}</v-card-text>

      <v-divider></v-divider>

      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="primary" flat @click="dialog = false">Done</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  props: ["title", "description"],
  data() {
    return {
      dialog: false
    };
  }
};
</script>
