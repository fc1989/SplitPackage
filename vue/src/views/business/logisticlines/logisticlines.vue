<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newLogisticLineRule"
        :editRule="logisticLineRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('LogisticLines.LineName')" prop="lineName">
                        <Input v-model="slotProps.createModel.lineName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('LogisticLines.LineCode')" prop="lineCode">
                        <Input v-model="slotProps.createModel.lineCode" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Logistics')" prop="logisticId">
                        <Input v-model="slotProps.createModel.logisticId"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('LogisticLines.LineName')" prop="lineName">
                        <Input v-model="slotProps.editModel.lineName" :maxlength="200" :minlength="1"></Input>
                    </FormItem>
                    <FormItem :label="$t('LogisticLines.LineCode')" prop="lineCode">
                        <Input v-model="slotProps.editModel.lineCode" :maxlength="50"></Input>
                    </FormItem>
                    <FormItem :label="$t('Logistics')" prop="logisticId">
                        <Input v-model="slotProps.editModel.logisticId"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import LogisticLineApi from "@/api/logisticline";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "LogisticLines",
      api: LogisticLineApi,
      newLogisticLineRule: {
        lineName: [{ require: true }],
        lineCode: [{ require: true }],
        logisticId: [{ require: true }]
      },
      logisticLineRule: {
        lineName: [{ require: true }],
        lineCode: [{ require: true }],
        logisticId: [{ require: true }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.$t('LogisticLines.LineName'),
            key: "lineName"
          },
          {
            title: this.$t('LogisticLines.LineCode'),
            key: "lineCode"
          },
          {
            title: this.$t('Logistics'),
            key: "logisticId"
          },
          {
            title: this.$t('Public.IsActive'),
            render: (h, params) => {
              return h("Checkbox", {
                props: {
                  value: params.row.isActive,
                  disabled: true
                }
              });
            }
          }
        ]
      }
    };
  }
};
</script>