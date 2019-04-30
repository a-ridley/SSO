<template>
  <div>
    <v-card lg12>
      <v-layout row wrap>
        <v-flex lg9>
          <h1 id="appPortal">Application Portal</h1>
        </v-flex>
        <v-flex lg3 id="appFilters">
          <v-select @change="filterApps" :items="sortBy" label="Sort By"></v-select>
        </v-flex>
      </v-layout>

      <v-container fluid grid-list-md>
        <v-layout row wrap>
          <v-flex xs12 md6 lg6 v-for="(app, index) in applications" :key="index">
            <!-- The card that shows up if the app is under maintenance -->
            <v-card v-if="app.UnderMaintenance || healthCheck.HealthStatuses[app.Id]" class="transparent">
              <v-card-title primary-title>
                <!-- If there is no logo, then a default image will be shown -->
                <img v-if="app.LogoUrl === null" src="@/assets/no-image-icon.png">
                <img v-else :src="app.LogoUrl">

                <div class="content">
                  <!-- Launching to an app can be done by clicking the app title -->
                  <h3 class="headline mb-0">
                    <strong class="truncate">{{ app.Title | truncate(maxTitleLength, textTail)}}</strong>
                  </h3>
                </div>
              </v-card-title>
              <div class="appInfo">
                <AppDetails v-if="app.Description != ''" :title="app.Title" :description="app.Description" />
                <AppDetails v-else :title="app.Title" :description="defaultDescription" />

                <v-chip color="indigo" text-color="white">Popularity: {{ app.ClickCount | truncate(maxPopularityLength, textTail)}}</v-chip>

                <v-chip color="orange" text-color="white">
                  Under Maintenance
                  <v-icon right large>build</v-icon>
                </v-chip>
              </div>
            </v-card>

            <!-- The card that shows up if the app is not under maintenance -->
            <v-card v-else hover>
              <v-card-title primary-title>
                <!-- If there is no logo, then a default image will be shown -->
                <img v-if="app.LogoUrl === null" src="@/assets/no-image-icon.png" @click="launch(app.Id, app)">
                <img v-else :src="app.LogoUrl" @click="launch(app.Id, app)">
                <div id="content" v-if="app.UnderMaintenance || healthCheck.HealthStatuses[app.Id]">
                  <!-- Launching to an app can be done by clicking the app title -->
                  <h3 class="headline mb-0" row wrap>
                    <strong>{{ app.Title | truncate(maxTitleLength, textTail)}}</strong>
                  </h3>
                </div>
                <div class="content" v-else>
                  <!-- Launching to an app can be done by clicking the app title -->
                  <h3 id="launchable" class="headline mb-0" @click="launch(app.Id, app)">
                    <strong>{{ app.Title | truncate(maxTitleLength, textTail)}}</strong>
                  </h3>
                </div>
              </v-card-title>
              <div class="appInfo">
                <AppDetails :title="app.Title" :description="app.Description" />
                <v-chip color="indigo" text-color="white">Popularity: {{ app.ClickCount | truncate(maxPopularityLength, textTail)}}</v-chip>
              </div>
            </v-card>
            <!-- Loads only if app is in progress of launching -->
            <div v-if="launchLoading">
              <Loading :dialog="launchLoading" />
            </div>
          </v-flex>
        </v-layout>
      </v-container>
    </v-card>

    <v-alert :value="error" type="error" transition="scale-transition">{{error}}</v-alert>
  </div>
</template>

<script>
import Vue from "vue";
import Loading from "@/components/Dialogs/Loading.vue";
import { signAndLaunch } from "@/services/oauth";
import AppDetails from "@/components/Dialogs/AppDetails.vue";
import { filter } from "@/services/TextFormat";
import { apiURL } from "@/const.js";
import axios from "axios";

Vue.filter("truncate", filter);

export default {
  components: { Loading, AppDetails },
  data() {
    return {
      text: "This Is An Extremely Long Application Title",
      textTail: "...",
      maxPopularityLength: 5,
      applications: [],
      sortBy: [
        "Alphabetical (Ascending)",
        "Alphabetical (Descending)",
        "Popularity",
        "Number of logins"
      ],
      defaultDescription:
        "No Description. Sirloin short loin tenderloin tri-tip jowl chicken shank ribeye landjaeger, pancetta pork chop. Cupim filet mignon tail porchetta, biltong leberkas turkey flank pork chop frankfurter kevin short loin tenderloin tri-tip shankle. Porchetta boudin shoulder sausage, beef ribs pancetta burgdoggen prosciutto tongue. Sausage kevin strip steak, pork belly pig filet mignon chuck shankle andouille tri-tip ham cow. Pork loin t-bone doner, kevin jowl cupim sausage meatloaf.",
      launchLoading: false,
      maintenance: false,
      currentPage: 1,
      pageSize: 20,
      healthCheck: {
        LastHealthCheck: new Date(),
        HealthStatuses: {}
      },
      error: ""
    };
  },
  computed: {
    maxTitleLength: function() {
      var maxTitleLength = 0;
      if (window.innerWidth > 414) maxTitleLength = 25;
      else maxTitleLength = 16;
      return maxTitleLength;
    }
  },
  methods: {
    launch(appId, app) {
      this.error = "";

      this.updateClickCount(app);

      this.launchLoading = true;

      signAndLaunch(appId)
        .catch(e => {
          this.error = e.message;
        })
        .finally(() => {
          this.launchLoading = false;
        });
    },
    filterApps: function(value) {
      if (value === this.sortBy[0]) this.sortByAscending();
      else if (value === this.sortBy[1]) this.sortByDescending();
      else if (value === this.sortBy[2]) this.sortByNumberOfClicks();
      else alert("Coming Soon!");
    },
    async sortByAscending() {
      await axios
        .get(`${apiURL}/applications/ascending`)
        .then(response => (this.applications = response.data));
    },
    async sortByDescending() {
      await axios
        .get(`${apiURL}/applications/descending`)
        .then(response => (this.applications = response.data));
    },
    async sortByNumberOfClicks() {
      await axios
        .get(`${apiURL}/applications/clicks`)
        .then(response => (this.applications = response.data));
    },
    async updateClickCount(app) {
      app.ClickCount += 1;
      await axios.post(`${apiURL}/applications/update`, {
        Title: app.Title,
        Email: app.Email,
        Description: app.Description,
        LogoUrl: app.LogoUrl,
        UnderMaintenance: app.UnderMaintenance,
        ClickCount: app.ClickCount,
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json"
        }
      });
    }
  },
  async created() {
    await axios
      .get(`${apiURL}/applications`, {
        params: {
          currentPage: this.currentPage,
          pageSize: this.pageSize
        }
      })
      .then(response => {
        this.applications = response.data;
        this.applications.forEach(app => {
          this.healthCheck.HealthStatuses[app.Id] = false;
        });
      });
  },
  mounted() {
    setInterval(() => {
      axios
        .get(`${apiURL}/applications/healthcheck`)
        .then(response => (this.healthCheck = response.data))
        .catch("Unexpected error occured in the server.");
    }, 10000);
  }
};
</script>

<style scoped>
.v-card {
  margin: 1em;
}

#appPortal {
  padding: 1em 1em 0 1em;
  font-size: 38px;
  text-decoration: underline;
}

.content {
  margin-left: 1em;
  margin-right: 1em;
}

#appFilters {
  padding: 2em 3em 0 2em;
}

.appInfo {
  padding-bottom: 1em;
  margin: 0 1em;
}

#launchable:hover {
  text-decoration: underline;
}

.transparent {
  background-color: white !important;
  opacity: 0.65;
  border-color: transparent !important;
}
</style>
