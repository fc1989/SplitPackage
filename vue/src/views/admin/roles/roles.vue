<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newRoleRule"
        :editRule="roleRule">
        <template slot="newform" slot-scope="slotProps">
            <FormItem :label="$t('Roles.RoleName')" prop="name">
                <Input v-model="slotProps.createModel.name" :maxlength="32" :minlength="2"></Input>
            </FormItem>
            <FormItem :label="$t('Roles.DisplayName')" prop="displayName">
                <Input v-model="slotProps.createModel.displayName" :maxlength="32" :minlength="2"></Input>
            </FormItem>
            <FormItem :label="$t('Roles.RoleDescription')" prop="description">
                <Input v-model="slotProps.createModel.description"></Input>
            </FormItem>
            <FormItem :label="$t('Roles.Permissions')">
                <CheckboxGroup v-model="slotProps.createModel.permissions">
                    <Checkbox :label="permission.name" v-for="permission in permissions" :key="permission.name"><span>{{permission.displayName}}</span></Checkbox>
                </CheckboxGroup>
            </FormItem>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <FormItem :label="$t('Roles.RoleName')" prop="name">
                <Input v-model="slotProps.editModel.name" :maxlength="32" :minlength="2"></Input>
            </FormItem>
            <FormItem :label="$t('Roles.DisplayName')" prop="displayName">
                <Input v-model="slotProps.editModel.displayName" :maxlength="32" :minlength="2"></Input>
            </FormItem>
            <FormItem :label="$t('Roles.RoleDescription')" prop="description">
                <Input v-model="slotProps.editModel.description"></Input>
            </FormItem>
            <FormItem :label="$t('Roles.Permissions')">
                <CheckboxGroup v-model="slotProps.editModel.permissions">
                    <Checkbox :label="permission.name" v-for="permission in permissions" :key="permission.name"><span>{{permission.displayName}}</span></Checkbox>
                </CheckboxGroup>
            </FormItem> 
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import RoleApi from "@/api/role";

export default {
  components: {
    simplePage
  },
  async created() {
    let rep = await RoleApi.GetAllPermissions();
    this.allPermissions = rep.data.result.items;
  },
  computed: {
    permissions() {
      return this.allPermissions;
    }
  },
  data() {
    return {
      title: "Menu.Pages.Roles",
      allPermissions: [],
      api: RoleApi,
      newRoleRule: {
        name: [{ required: true, trigger: "blur" }],
        displayName: [{ required: true, trigger: "blur" }]
      },
      roleRule: {
        name: [{ required: true, trigger: "blur" }],
        displayName: [{ required: true, trigger: "blur" }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.$t("Roles.RoleName"),
            key: "name"
          },
          {
            title: this.$t("Roles.DisplayName"),
            key: "displayName"
          }
        ]
      }
    };
  }
};
</script>