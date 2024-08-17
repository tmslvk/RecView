<template>
  <div class="card">
    <nav
      class="navbar"
      role="navigation"
      aria-label="main navigation"
    >
      <div class="navbar-brand">
        <a
          class="navbar-item"
          href="https://bulma.io"
        >
          <img
            src="https://bulma.io/images/bulma-logo.png"
            width="112"
            height="28"
          >
        </a>

        <a
          role="button"
          class="navbar-burger"
          aria-label="menu"
          aria-expanded="false"
          data-target="navbarBasicExample"
        >
          <span aria-hidden="true"></span>
          <span aria-hidden="true"></span>
          <span aria-hidden="true"></span>
        </a>
      </div>

      <div
        id="navbarBasicExample"
        class="navbar-menu"
      >
        <div class="navbar-start">
          <router-link
            class="navbar-item"
            to="/MainPage"
          >
            Home
          </router-link>
          <router-link
            class="navbar-item"
            to="/Profile"
          >
            Profile
          </router-link>

          <router-link
            class="navbar-item"
            to="/Bands"
          >
            Bands
          </router-link>

          <div class="navbar-item has-dropdown is-hoverable">
            <a class="navbar-link">
              More
            </a>

            <div class="navbar-dropdown">
              <a class="navbar-item">
                About
              </a>
              <a class="navbar-item">
                Jobs
              </a>
              <a class="navbar-item">
                Contact
              </a>
              <hr class="navbar-divider">
              <a class="navbar-item">
                Report an issue
              </a>
            </div>
          </div>
        </div>

        <div class="navbar-end">
          <div class="navbar-item">
            <div class="dropdown is-hoverable is-right">
              <div class="dropdown-trigger">
                <button
                  class="button"
                  aria-haspopup="true"
                  aria-controls="dropdown-menu3"
                >
                  <span class="icon is-small">
                    <i class="fa-solid fa-user"></i>
                  </span>
                  <span>{{this.$store.state.user.username}}</span>
                </button>
              </div>
              <div
                class="dropdown-menu"
                id="dropdown-menu3"
                role="menu"
              >
                <div class="dropdown-content">
                  <div class="dropdown-item">
                    <div class="block">
                      <div class="title">Info</div>
                      <div class="is-size-5">
                        <strong>Profile</strong>
                      </div>
                      <h1><strong>{{this.$store.state.user.username}}</strong></h1>
                      <h1>{{this.$store.state.user.firstname}} {{this.$store.state.user.lastname}}</h1>
                      <h1>{{(new Date(this.$store.state.user.dateOfBirth)).toLocaleDateString('en-GB')}}</h1>
                      <div v-if="this.$store.state.user.musician">
                        <div class="is-size-5">
                          <strong>Musician</strong>
                        </div>
                        <h1>{{this.$store.state.user.musician.profileInstruments}}/{{this.$store.state.user.musician.yearsOfExperience}} years</h1>
                        <h1>{{this.$store.state.user.musician.statusOfActivity}}</h1>
                        <h1>{{this.$store.state.user.musician.country}}, {{this.$store.state.user.musician.city}}</h1>
                      </div>
                    </div>
                  </div>
                  <div class="dropdown-divider">
                    <div class="dropdown-content">
                      <div class="dropdown-item">
                        <router-link
                          class="navbar-item"
                          to="/Musician"
                        >
                          <strong>Become a musician</strong>
                        </router-link>
                        <router-link
                          class="navbar-item"
                          to="/Login"
                        >
                          <strong>Log out</strong>
                        </router-link>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </nav>
  </div>
</template>

<script>
</script>

<style scoped>
</style>
